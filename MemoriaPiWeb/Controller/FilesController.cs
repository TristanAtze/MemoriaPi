using MemoriaPiDataCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaPiWeb.Controller
{
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

        [HttpPost("Upload")]
        // Das [ValidateAntiForgeryToken] wird hier entfernt, da [Authorize] ausreicht.
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
            Directory.CreateDirectory(userFolderPath);

            long currentSize = GetDirectorySize(userFolderPath);
            long uploadSize = files.Sum(f => f.Length);
            long allowedSize = (long)user.StorageCapacityGB * 1024 * 1024 * 1024;

            if (currentSize + uploadSize > allowedSize)
            {
                return BadRequest(new { message = "Nicht genügend Speicherplatz." });
            }

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(userFolderPath, formFile.FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        // Optional: Umgang mit bereits existierenden Dateien
                        return BadRequest(new { message = $"Die Datei '{formFile.FileName}' existiert bereits." });
                    }
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new { message = $"{files.Count} Datei(en) erfolgreich hochgeladen." });
        }

        // NEU: Methode zum Erstellen eines Ordners
        [HttpPost("CreateFolder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFolder([FromBody] FileActionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
            var newFolderPath = Path.Combine(userFolderPath, request.FolderName);

            if (Directory.Exists(newFolderPath))
            {
                return BadRequest(new { message = "Ein Ordner mit diesem Namen existiert bereits." });
            }

            Directory.CreateDirectory(newFolderPath);
            return Ok(new { message = "Ordner erfolgreich erstellt." });
        }

        // ... (Ihre bestehenden Delete- und Rename-Methoden für Dateien) ...

        private long GetDirectorySize(string path)
        {
            // ...
        }

        public class FileActionRequest
        {
            public string FileName { get; set; }
            public string NewFileName { get; set; }
            public string FolderName { get; set; } // Hinzugefügt für Ordner-Aktionen
        }
    }
}