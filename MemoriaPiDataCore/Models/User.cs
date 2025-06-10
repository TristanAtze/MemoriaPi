namespace MemoriaPiDataCore.Models
{
    public class User
    {
        public int Id { get; set; }
        public int UserRoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // In einem echten Projekt sollte dies ein Hash sein!
        public bool HasAccess { get; set; }

        // Navigation property to hold the related UserRole object
        public UserRole? Role { get; set; }

        public User(int id, int userRoleId, string name, string email, string userName, string password, bool hasAccess)
        {
            Id = id;
            UserRoleId = userRoleId;
            Name = name;
            Email = email;
            UserName = userName;
            Password = password;
            HasAccess = hasAccess;
        }
    }
}