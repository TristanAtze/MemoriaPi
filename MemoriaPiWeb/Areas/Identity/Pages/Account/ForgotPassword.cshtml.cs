#nullable disable

using MemoriaPiDataCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MemoriaPiWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool FormSubmittedSuccessfully { get; set; } = false;
        public string DebugLink { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // *** KORREKTUR HIER: Wir verwenden FirstOrDefaultAsync, um den Fehler abzufangen ***
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);

                // Wir prüfen nur noch, ob der Benutzer existiert.
                // Der Email-Bestätigungs-Check ist für die Entwicklung entfernt.
                if (user == null)
                {
                    // Zeige die Erfolgsmeldung trotzdem an, um nicht preiszugeben, ob ein Benutzer existiert.
                    FormSubmittedSuccessfully = true;
                    return Page();
                }

                // Dieser Code wird jetzt für jeden existierenden Benutzer erreicht.
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                FormSubmittedSuccessfully = true;
                DebugLink = callbackUrl;
            }

            return Page();
        }
    }
}