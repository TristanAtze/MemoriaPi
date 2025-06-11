using MemoriaPiDataCore.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoriaPiDataCore.SQL
{
    public class DatabaseSetup
    {
        private readonly string _connectionString;

        //Server=(localdb)\\LocalTestDB;Database=TestDB;Integrated Security=True;)

        public DatabaseSetup(string connectionString = "Data Source = 192.168.6.131; Initial Catalog = _BS_TestDB; User ID = Azubi; Password = TestSQL2020#!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultipleActiveResultSets=True;")
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