using Microsoft.AspNetCore.Identity;
using MemoriaPiDataCore.Models;

namespace MemoriaPiWeb.Middleware
{
    public class CheckPasswordResetMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckPasswordResetMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            // Nur für angemeldete Benutzer prüfen
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(context.User);

                // Prüfen, ob der Benutzer das Flag hat UND sich nicht bereits auf der Seite zur Passwortänderung befindet
                if (user != null && user.MustChangePassword && !context.Request.Path.Value.Contains("/Account/Manage/ChangePassword"))
                {
                    // Den Benutzer abmelden und zur Login-Seite weiterleiten mit einer Nachricht
                    await signInManager.SignOutAsync();
                    context.Response.Redirect("/Identity/Account/Login?mustChangePassword=true");
                    return; // Wichtig: Die weitere Ausführung stoppen
                }
            }

            await _next(context); // Nächste Middleware in der Kette aufrufen
        }
    }
}