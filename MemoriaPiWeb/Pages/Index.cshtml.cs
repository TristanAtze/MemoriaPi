using MemoriaPiDataCore.Models;
using MemoriaPiWeb.Services;
using Microsoft.AspNetCore.Authorization; // Diese Zeile hinzuf�gen
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MemoriaPiWeb.Pages
{
    [Authorize] // DIESES ATTRIBUT SCH�TZT DIE SEITE
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ActiveUserStore _activeUserStore;

        public List<ApplicationUser> ActiveUsers { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, ActiveUserStore activeUserStore)
        {
            _logger = logger;
            _activeUserStore = activeUserStore;
        }

        public void OnGet()
        {
            ActiveUsers = _activeUserStore.GetActiveUsers();
        }
    }
}