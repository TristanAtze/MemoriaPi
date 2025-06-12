using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MemoriaPiWeb.Pages
{
    [Authorize] // Stellt sicher, dass nur angemeldete Benutzer diese Seite sehen
    public class CloudModel : PageModel
    {
        private readonly ILogger<CloudModel> _logger;

        public CloudModel(ILogger<CloudModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Hier wird später die Logik zum Anzeigen von Dateien und Ordnern hinkommen.
        }
    }
}