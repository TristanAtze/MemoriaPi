#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MemoriaPiDataCore.Models;
using System.IO; // Hinzufügen
using Microsoft.AspNetCore.Hosting; // Hinzufügen

namespace MemoriaPiWeb.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment; // Hinzufügen

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment) // Hinzufügen
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment; // Hinzufügen
        }

        public string Username { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            // NEU: Eigenschaft für das Profilbild
            [Display(Name = "Profile Picture")]
            public IFormFile ProfilePicture { get; set; }
        }

        public string CurrentProfilePictureUrl { get; set; } // Hinzufügen

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            CurrentProfilePictureUrl = user.ProfilePictureUrl; // Hinzufügen

            Input = new InputModel
            {
                Username = userName,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Benutzernamen und Telefonnummer aktualisieren (wie bisher)
            // ...

            // NEU: Logik zum Verarbeiten des Bild-Uploads
            if (Input.ProfilePicture != null)
            {
                // Eindeutigen Dateinamen erstellen, um Konflikte zu vermeiden
                string fileName = user.Id + Path.GetExtension(Input.ProfilePicture.FileName);
                // Pfad zum Ordner, in dem die Bilder gespeichert werden (wwwroot/images/profile)
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profile");
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Sicherstellen, dass der Ordner existiert
                Directory.CreateDirectory(uploadsFolder);

                // Altes Bild löschen, falls vorhanden
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePictureUrl = $"/images/profile/{fileName}";
                await _userManager.UpdateAsync(user);
            }


            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}