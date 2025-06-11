using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using MemoriaPiDataCore.Models; // Wichtig für ApplicationUser
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MemoriaPiWeb.Pages
{
    [Authorize] // Stellt sicher, dass nur angemeldete Benutzer die Seite sehen
    public class DashboardModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // Die Eigenschaft, die von deiner .cshtml-Datei verwendet wird
        public bool IsAdmin { get; private set; }

        // Der Konstruktor wird erweitert, um den UserManager zu erhalten
        public DashboardModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Diese Methode wird aufgerufen, wenn die Seite geladen wird
        public async Task<IActionResult> OnGetAsync()
        {
            // Holt den aktuell angemeldeten Benutzer
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Sollte nicht passieren, wenn [Authorize] verwendet wird, aber sicher ist sicher
                return NotFound($"Benutzer konnte nicht geladen werden mit ID '{_userManager.GetUserId(User)}'.");
            }

            // HIER IST DIE KORREKTE PRÜFUNG:
            // Wir prüfen, ob der Benutzer in der Rolle "Admin" (großgeschrieben!) ist
            // und setzen unsere Eigenschaft entsprechend.
            IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            return Page();
        }
    }
}