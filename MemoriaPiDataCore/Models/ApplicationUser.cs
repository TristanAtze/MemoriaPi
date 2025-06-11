using Microsoft.AspNetCore.Identity;

namespace MemoriaPiDataCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }
        public bool HasAccess { get; set; } = false;
        public string? ProfilePictureUrl { get; set; }
    }
}