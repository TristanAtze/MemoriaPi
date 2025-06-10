using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MemoriaPiDataCore.Models;
using Microsoft.EntityFrameworkCore; 

public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // So einfach bekommen Sie jetzt alle Benutzer aus der Identity-Tabelle
        List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();

        // Jetzt können Sie die Benutzer an eine View übergeben
        return View(allUsers);
    }
}