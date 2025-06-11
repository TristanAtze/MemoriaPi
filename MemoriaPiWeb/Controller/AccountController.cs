//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using MemoriaPiDataCore.Models;
//using System.Threading.Tasks;

//namespace MemoriaPiDataCore.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;

//        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//        }

//        // GET: /Account/Register
//        [HttpGet]
//        public IActionResult Register()
//        {
//            // Leitet zur Razor Page für die Registrierung weiter
//            return RedirectToPage("/Account/Register", new { area = "Identity" });
//        }

//        // POST: /Account/Register - Dieser Endpunkt wird von der Razor Page übernommen,
//        // aber wir lassen ihn hier, falls Sie ihn später benötigen.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Register(RegisterViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                // *** KORREKTUR HIER: Die 'Password'-Zuweisung wurde entfernt ***
//                var user = new ApplicationUser
//                {
//                    UserName = model.Email,
//                    Email = model.Email,
//                    HasAccess = false // Standardmäßig auf false gesetzt
//                };

//                // Der UserManager kümmert sich um das Passwort
//                var result = await _userManager.CreateAsync(user, model.Password);

//                if (result.Succeeded)
//                {
//                    // Leitet zur Razor Page für den Login weiter
//                    return RedirectToPage("/Account/Login", new { area = "Identity" });
//                }

//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError(string.Empty, error.Description);
//                }
//            }
//            // Wenn etwas fehlschlägt, bleiben wir auf der Registrierungs-Seite
//            return RedirectToPage("/Account/Register", new { area = "Identity" });
//        }

//        // GET: /Account/Login
//        [HttpGet]
//        public IActionResult Login()
//        {
//            // Leitet zur Razor Page für den Login weiter
//            return RedirectToPage("/Account/Login", new { area = "Identity" });
//        }

//        // POST: /Account/Login - Dieser Endpunkt wird von der Razor Page übernommen.

//        // GET: /Account/AccessDenied
//        [HttpGet]
//        public IActionResult AccessDenied()
//        {
//            // Leitet zur Razor Page für "Zugriff verweigert" weiter
//            return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
//        }

//        // POST: /Account/Logout
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Logout()
//        {
//            await _signInManager.SignOutAsync();
//            return RedirectToAction("Index", "Home");
//        }
//    }
//}