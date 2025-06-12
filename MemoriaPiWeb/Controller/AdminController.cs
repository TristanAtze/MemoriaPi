using MemoriaPiDataCore.Models;
using MemoriaPiWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MemoriaPiWeb.Controller
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    CurrentRoles = roles
                });
            }
            return View(userViewModels);
        }

        // GET: /Admin/Edit/userId
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                MustChangePassword = user.MustChangePassword,
                IsLockedOut = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow,
                AllRoles = allRoles.Select(r => new SelectListItem(r.Name, r.Name)).ToList(),
                SelectedRoles = userRoles.ToList()
            };

            return View(model);
        }

        // POST: /Admin/Edit/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.MustChangePassword = model.MustChangePassword;

            // Profilbild-Upload verarbeiten
            if (model.NewProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/profile-pictures");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewProfilePicture.CopyToAsync(fileStream);
                }
                user.ProfilePictureUrl = "/images/profile-pictures/" + uniqueFileName;
            }

            // Rollen aktualisieren
            var currentRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!result.Succeeded) return View(model); // Fehlerbehandlung

            result = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
            if (!result.Succeeded) return View(model); // Fehlerbehandlung


            // Account sperren/entsperren
            if (model.IsLockedOut)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
            else if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }


        // POST: /Admin/Delete/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Verhindere, dass der letzte Admin sich selbst löscht
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin && (await _userManager.GetUsersInRoleAsync("Admin")).Count == 1)
                {
                    TempData["ErrorMessage"] = "Der letzte Administrator kann nicht gelöscht werden.";
                    return RedirectToAction(nameof(Index));
                }

                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}