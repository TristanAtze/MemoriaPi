using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MemoriaPiDataCore.Models;
using System;
using System.Threading.Tasks;

namespace MemoriaPiDataCore.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Rollen erstellen, falls sie nicht existieren
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = await userManager.FindByEmailAsync("admin@memoriapi.com");
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    // --- ÄNDERUNG HIER ---
                    UserName = "admin@memoriapi.com", // Benutzername ist jetzt die E-Mail
                    Email = "admin@memoriapi.com",
                    EmailConfirmed = true,
                    HasAccess = true
                };
                var result = await userManager.CreateAsync(newAdminUser, "AdminPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }
        }
    }
}