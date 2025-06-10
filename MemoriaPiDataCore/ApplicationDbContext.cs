using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MemoriaPiDataCore.Models;

namespace MemoriaPiDataCore.Data
{
    // Sicherstellen, dass der DbContext von IdentityDbContext<User> erbt
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}