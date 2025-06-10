#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MemoriaPiWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmationModel : PageModel
    {
        // Diese Eigenschaft wird den Link für die Ansicht speichern
        public string DebugLink { get; set; }

        public void OnGet()
        {
            // Wir lesen den Wert aus TempData aus.
            // TempData wird nach dem Lesen automatisch geleert.
            if (TempData.ContainsKey("DebugLink"))
            {
                DebugLink = TempData["DebugLink"].ToString();
            }
        }
    }
}