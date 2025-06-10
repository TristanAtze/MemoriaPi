using MemoriaPiDataCore.Models;
using Microsoft.Data.SqlClient;
using MemoriaPiCrypto;

namespace MemoriaPiDataCore.SQL
{
    public class SqlWriter
    {
        private readonly string _connectionString;

        public SqlWriter(string connectionString = "Server=(localdb)\\LocalTestDB;Database=TestDB;Integrated Security=True;")
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
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@userName", user.UserName);
                command.Parameters.AddWithValue("@password", user.UserName); 
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