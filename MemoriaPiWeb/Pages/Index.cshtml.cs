using MemoriaPiDataCore.Models; // Hinzuf�gen
using MemoriaPiWeb.Services; // Hinzuf�gen
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MemoriaPiWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ActiveUserStore _activeUserStore; // Hinzuf�gen

        // Property f�r die Ansicht
        public List<ApplicationUser> ActiveUsers { get; private set; }

        // Konstruktor anpassen
        public IndexModel(ILogger<IndexModel> logger, ActiveUserStore activeUserStore)
        {
            _logger = logger;
            _activeUserStore = activeUserStore; // Hinzuf�gen
        }

        public void OnGet()
        {
            // Aktive Benutzer f�r die Anzeige auf der Seite abrufen
            ActiveUsers = _activeUserStore.GetActiveUsers();
        }
    }
}