using MemoriaPiDataCore.Models; // Hinzufügen
using MemoriaPiWeb.Services; // Hinzufügen
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MemoriaPiWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ActiveUserStore _activeUserStore; // Hinzufügen

        // Property für die Ansicht
        public List<ApplicationUser> ActiveUsers { get; private set; }

        // Konstruktor anpassen
        public IndexModel(ILogger<IndexModel> logger, ActiveUserStore activeUserStore)
        {
            _logger = logger;
            _activeUserStore = activeUserStore; // Hinzufügen
        }

        public void OnGet()
        {
            // Aktive Benutzer für die Anzeige auf der Seite abrufen
            ActiveUsers = _activeUserStore.GetActiveUsers();
        }
    }
}