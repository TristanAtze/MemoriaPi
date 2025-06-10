using Microsoft.AspNetCore.Identity;

namespace MemoriaPiDataCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Ein leerer Konstruktor für EF Core.
        public ApplicationUser() { }

        public bool HasAccess { get; set; } = false;
    }
}