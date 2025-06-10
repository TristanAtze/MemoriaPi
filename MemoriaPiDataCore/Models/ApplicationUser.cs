using Microsoft.AspNetCore.Identity;

namespace MemoriaPiDataCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Ein leerer Konstruktor f�r EF Core.
        public ApplicationUser() { }

        public bool HasAccess { get; set; } = false;
    }
}