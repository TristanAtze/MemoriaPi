// Dies sind die notwendigen using-Anweisungen am Anfang der Datei.
// Stelle sicher, dass sie alle vorhanden sind.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MemoriaPiDataCore.Models; // Wichtig, um deine ApplicationUser-Klasse zu kennen

namespace MemoriaPiWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        // Hinzufügen des UserManager und korrekte Initialisierung im Konstruktor
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
                          ILogger<LoginModel> logger,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            // Das Label im UI kann "E-Mail-Adresse" bleiben, aber es funktioniert jetzt für beides
            [Required]
            [Display(Name = "Benutzername oder E-Mail")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Passwort")]
            public string Password { get; set; }

            [Display(Name = "Angemeldet bleiben?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        // Die korrigierte OnPostAsync-Methode
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // **KORRIGIERTE LOGIK**
                var userNameOrEmail = Input.Email;

                // Versuche, den Benutzer anhand des Namens ODER der E-Mail zu finden
                var user = await _userManager.FindByNameAsync(userNameOrEmail)
                        ?? await _userManager.FindByEmailAsync(userNameOrEmail);

                if (user != null)
                {
                    // Wichtig: Wir verwenden den gefundenen Benutzernamen für den Login-Versuch.
                    // Die Variable 'result' wird hier deklariert, damit sie im gesamten Block gültig ist.
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                }

                // Diese Zeile wird erreicht, wenn user == null ODER das Passwort falsch ist.
                ModelState.AddModelError(string.Empty, "Ungültiger Anmeldeversuch.");
                return Page();
            }

            // Wenn das ModelState ungültig ist, kehren wir zur Seite zurück.
            return Page();
        }
    }
}