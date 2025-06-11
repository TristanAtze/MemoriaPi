using MemoriaPiDataCore.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoriaPiDataCore.SQL
{
    public class DatabaseSetup
    {
        private readonly string _connectionString;

        //Server=(localdb)\\LocalTestDB;Database=TestDB;Integrated Security=True;)

        public DatabaseSetup(string connectionString = "Server=(localdb)\\LocalTestDB;Database=TestDB;Integrated Security=True;")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
            }
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_connectionString);

            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Diese Zeile ist alles, was wir brauchen.
                // Sie erstellt die Datenbank UND wendet alle Migrationen an.
                context.Database.Migrate();
            }
        }
    }
}