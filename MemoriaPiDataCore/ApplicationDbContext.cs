using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MemoriaPiDataCore.Models;

// Der Namespace ist jetzt der korrekte "Pfad" zur Datei.
namespace MemoriaPiDataCore.Data
{
    // Dies ist die Klasse innerhalb des Data-Namespaces.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}