using MemoriaPiDataCore.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace MemoriaPiDataCore.SQL
{
    public class SqlReader
    {
        private readonly string _connectionString;
        
        private static List<User> _cachedUsers = new List<User>();
        private static List<UserRole> _cachedRoles = new List<UserRole>();

        public SqlReader(string connectionString = "Data Source = 192.168.6.131; Initial Catalog = _BS_TestDB; User ID = Azubi; Password = TestSQL2020#!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultipleActiveResultSets=True;")
        {
            _connectionString = connectionString;
            ReloadAllData();
        }
        
        public static List<User> GetCachedUsers() => _cachedUsers;
        public static List<UserRole> GetCachedRoles() => _cachedRoles;
        
        public void ReloadAllData()
        {
            _cachedRoles = LoadRolesFromDb();
            _cachedUsers = LoadUsersFromDb();
        }

        private List<UserRole> LoadRolesFromDb()
        {
            var roles = new List<UserRole>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT ID, Role FROM UserRole;";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new UserRole
                        {
                            Id = reader.GetInt32(0),
                            Role = reader.GetString(1)
                        });
                    }
                }
            }
            return roles;
        }

        private List<User> LoadUsersFromDb()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT u.ID, u.UserRoleID, u.Name, u.Email, u.UserName, u.Password, u.HasAccess, ur.Role
                    FROM [User] u
                    INNER JOIN UserRole ur ON u.UserRoleID = ur.ID;
                ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(
                            new User(reader.GetInt32(0), 
                                reader.GetInt32(1), 
                                reader.GetString(2), 
                                reader.GetString(3),
                                reader.GetString(4), 
                                reader.GetString(5),
                                reader.GetBoolean(6)));
                    }
                }
            }
            return users;
        }
    }
}