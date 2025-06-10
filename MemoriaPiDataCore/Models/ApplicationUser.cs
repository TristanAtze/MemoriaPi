using Microsoft.AspNetCore.Identity;

namespace MemoriaPiDataCore.Models
{
    public class ApplicationUser : IdentityUser
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

        public ApplicationUser(int id = 0, int userRoleId = 1, string name = "", string email = "", string userName = "", string password = "", bool hasAccess = false)
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