using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MemoriaPiDataCore.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MemoriaPiDataCore.Data.ApplicationDbContext>
    {
        public MemoriaPiDataCore.Data.ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MemoriaPiWeb"))
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<MemoriaPiDataCore.Data.ApplicationDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }

            builder.UseSqlServer(connectionString);

            return new MemoriaPiDataCore.Data.ApplicationDbContext(builder.Options);
        }
    }
}