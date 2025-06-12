using MemoriaPiDataCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities; // Wichtig für WebEncoders
using System.Text; // Wichtig für Encoding.UTF8

namespace MemoriaPiWeb.Middleware
{
    public class CheckPasswordResetMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckPasswordResetMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(context.User);

                if (user != null && user.MustChangePassword)
                {
                    var path = context.Request.Path.Value;
                    var allowedPaths = new[] { "/Identity/Account/ResetPassword", "/Identity/Account/Logout" };

                    if (path != null && !allowedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    {
                        // 1. Passwort-Reset-Token generieren.
                        var code = await userManager.GeneratePasswordResetTokenAsync(user);

                        // KORREKTUR HIER: Den Token in das von der Zielseite erwartete Base64Url-Format kodieren.
                        var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        // 3. Die URL mit dem korrekt kodierten Token erstellen.
                        var redirectUrl = $"/Identity/Account/ResetPassword?code={encodedCode}&email={Uri.EscapeDataString(user.Email!)}";

                        context.Response.Redirect(redirectUrl);
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}