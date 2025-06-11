#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MemoriaPiDataCore.Models;
using MemoriaPiDataCore.Data; // Hinzuf�gen f�r den DbContext

namespace MemoriaPiWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context; // DbContext hinzuf�gen

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context; // Zuweisen
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel { [Required][EmailAddress] public string Email { get; set; } }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    // Eine neue Anfrage erstellen und speichern
                    var request = new PasswordResetRequest { UserId = user.Id };
                    _context.PasswordResetRequests.Add(request);
                    await _context.SaveChangesAsync();
                }

                // Immer zur Best�tigungsseite weiterleiten
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}