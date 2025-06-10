using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

// Der Namespace bleibt derselbe
namespace MemoriaPiDataCore.Data
{
    /*
     * Diese Klasse wird NUR von den 'dotnet ef'-Werkzeugen zur Design-Zeit verwendet.
     */
    // Hier verwenden wir den vollqualifizierten Namen
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MemoriaPiDataCore.Data.ApplicationDbContext>
    {
        // Hier verwenden wir den vollqualifizierten Namen
        public MemoriaPiDataCore.Data.ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MemoriaPiWeb"))
                .AddJsonFile("appsettings.json")
                .Build();

            // Hier verwenden wir den vollqualifizierten Namen
            var builder = new DbContextOptionsBuilder<MemoriaPiDataCore.Data.ApplicationDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }

            builder.UseSqlServer(connectionString);

            // Hier verwenden wir den vollqualifizierten Namen
            return new MemoriaPiDataCore.Data.ApplicationDbContext(builder.Options);
        }
    }
}