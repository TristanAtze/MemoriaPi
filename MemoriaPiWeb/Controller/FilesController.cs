using System.IO;
using System.Threading.Tasks;
using MemoriaPiDataCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly UserManager<ApplicationUser> _userManager;

    public FilesController(IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
    {
        _hostingEnvironment = hostingEnvironment;
        _userManager = userManager;
    }

    // Upload-Methode (unverändert)
    [HttpPost("Upload")]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        // ... Ihr bestehender Upload-Code ...
        return Ok(new { message = $"{files.Count} Datei(en) erfolgreich hochgeladen." });
    }

    [HttpPost("Delete")]
    [ValidateAntiForgeryToken] // Optional, aber gut für die Sicherheit
    public async Task<IActionResult> Delete([FromBody] FileActionRequest request) // KORREKTUR HIER
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);

        // Wir verwenden jetzt request.FileName statt id
        var filePath = Path.Combine(userFolderPath, request.FileName);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            return Ok(new { message = "Datei erfolgreich gelöscht." });
        }
        return NotFound(new { message = "Datei nicht gefunden." });
    }

    // NEU: Methode zum Umbenennen einer Datei
    [HttpPost("Rename")]
    public async Task<IActionResult> Rename([FromBody] FileActionRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
        var oldFilePath = Path.Combine(userFolderPath, request.FileName);
        var newFilePath = Path.Combine(userFolderPath, request.NewFileName);

        if (System.IO.File.Exists(oldFilePath))
        {
            if (System.IO.File.Exists(newFilePath))
            {
                return BadRequest(new { message = "Eine Datei mit diesem Namen existiert bereits." });
            }
            System.IO.File.Move(oldFilePath, newFilePath);
            return Ok(new { message = "Datei erfolgreich umbenannt." });
        }
        return NotFound(new { message = "Datei nicht gefunden." });
    }


    private long GetDirectorySize(string path)
    {
        // ... Ihre bestehende Methode ...
        if (!Directory.Exists(path)) return 0;
        return new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
    }

    // NEU: Eine Klasse für die Anfragen
    public class FileActionRequest
    {
        public string FileName { get; set; }
        public string NewFileName { get; set; }
    }
}