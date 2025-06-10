using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize] // Schützt den gesamten Controller
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        // Hier kannst du zusätzlich prüfen, ob der User wirklich Zugriff hat,
        // falls du dies nicht schon in der Login-Logik tust.
        // Der einfachste Weg ist aber, den Zugriff direkt beim Login zu steuern.
        return View();
    }
}