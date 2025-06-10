using MemoriaPiDataCore.Models;
using Microsoft.Data.SqlClient;
using MemoriaPiCrypto;

namespace MemoriaPiDataCore.SQL
{
    public class SqlWriter
    {
        private readonly string _connectionString;

        public SqlWriter(string connectionString = "Data Source = 192.168.6.131; Initial Catalog = _BS_TestDB; User ID = Azubi; Password = TestSQL2020#!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultipleActiveResultSets=True;")
        {
            _connectionString = connectionString;
        }

        public void CreateUser(ApplicationUser user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO [User] (UserRoleID, [Name], Email, UserName, Password, HasAccess)
                    VALUES (@userRoleId, @name, @email, @userName, @password, @hasAccess);
                ";

                command.Parameters.AddWithValue("@userRoleId", user.UserRoleId);
                command.Parameters.AddWithValue("@name", Crypto.Encryption(user.Name));
                command.Parameters.AddWithValue("@email", Crypto.Encryption(user.Email));
                command.Parameters.AddWithValue("@userName", Crypto.Encryption(user.UserName));
                command.Parameters.AddWithValue("@password", Crypto.Encryption(user.UserName)); 
                command.Parameters.AddWithValue("@hasAccess", user.HasAccess);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateUser(ApplicationUser user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE [User]
                    SET UserRoleID = @userRoleId,
                        Name = @name,
                        Email = @email,
                        UserName = @userName,
                        Password = @password,
                        HasAccess = @hasAccess
                    WHERE ID = @id;
                ";

                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@userRoleId", user.UserRoleId);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@userName", user.UserName);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@hasAccess", user.HasAccess);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateUserField(int userId, string fieldName, object value)
        {
            var allowedFields = new List<string> { "UserRoleID", "Name", "Email", "UserName", "Password", "HasAccess" };
            if (!allowedFields.Contains(fieldName))
            {
                throw new ArgumentException($"Das Feld '{fieldName}' ist nicht zur Bearbeitung freigegeben.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                
                // Feldname wird sicher eingefügt, um SQL Injection zu vermeiden.
                command.CommandText = $"UPDATE [User] SET {fieldName} = @value WHERE ID = @id;";

                command.Parameters.AddWithValue("@id", userId);
                command.Parameters.AddWithValue("@value", value);

                command.ExecuteNonQuery();
            }
        }
    }
}