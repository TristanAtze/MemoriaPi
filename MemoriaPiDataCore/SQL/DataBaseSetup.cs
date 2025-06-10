using Microsoft.Data.SqlClient;
using System.Text;

namespace MemoriaPiDataCore.SQL
{
    public class DatabaseSetup
    {
        private readonly string _connectionString;
        private readonly string _masterConnectionString;
        private readonly string _dbName;

        public DatabaseSetup(string connectionString = "Data Source = 192.168.6.131; Initial Catalog = _BS_TestDB; User ID = Azubi; Password = TestSQL2020#!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultipleActiveResultSets=True;")
        {
            _connectionString = connectionString;
            
            var builder = new SqlConnectionStringBuilder(connectionString);
            _dbName = builder.InitialCatalog; 
            builder.InitialCatalog = "_BS_TestDB";
            _masterConnectionString = builder.ConnectionString;
        }

        public void InitializeDatabase()
        {
            EnsureDatabaseExists();
            EnsureTablesExist();
        }

        private void EnsureDatabaseExists()
        {
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"IF DB_ID(@dbName) IS NULL CREATE DATABASE [{_dbName}]";
                command.Parameters.AddWithValue("@dbName", _dbName);
                command.ExecuteNonQuery();
            }
        }

        private void EnsureTablesExist()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                var sb = new StringBuilder();

                // 1. Tabelle UserRole erstellen, wenn sie nicht existiert
                sb.AppendLine(@"
                    IF OBJECT_ID('UserRole', 'U') IS NULL
                    BEGIN
                        CREATE TABLE UserRole (
                            ID INT IDENTITY(1,1) PRIMARY KEY,
                            Role NVARCHAR(50) NOT NULL UNIQUE
                        );
                    END;
                ");
                
                // 2. Tabelle User erstellen, wenn sie nicht existiert
                sb.AppendLine(@"
                    IF OBJECT_ID('User', 'U') IS NULL
                    BEGIN
                        CREATE TABLE [User] (
                            ID INT IDENTITY(1,1) PRIMARY KEY,
                            UserRoleID INT NOT NULL,
                            Name NVARCHAR(255) NOT NULL,
                            Email NVARCHAR(255) UNIQUE,
                            UserName NVARCHAR(100) NOT NULL UNIQUE,
                            Password NVARCHAR(MAX) NOT NULL, -- Sollte ein Hash sein!
                            HasAccess BIT NOT NULL,
                            CONSTRAINT FK_User_UserRole FOREIGN KEY (UserRoleID) REFERENCES UserRole(ID)
                        );
                    END;
                ");

                // 3. Standard-Rollen einfügen, falls sie noch nicht existieren
                sb.AppendLine(@"
                    IF NOT EXISTS (SELECT 1 FROM UserRole WHERE Role = 'Admin')
                    BEGIN
                        INSERT INTO UserRole (Role) VALUES ('Admin');
                    END;
                    IF NOT EXISTS (SELECT 1 FROM UserRole WHERE Role = 'User')
                    BEGIN
                        INSERT INTO UserRole (Role) VALUES ('User');
                    END;
                ");
                
                command.CommandText = sb.ToString();
                command.ExecuteNonQuery();
            }
        }
    }
}