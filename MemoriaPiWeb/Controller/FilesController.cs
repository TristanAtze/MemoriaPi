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
        private readonly ILogger<FilesController> _logger;

        public FilesController(IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager, ILogger<FilesController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Lädt eine oder mehrere Dateien hoch.
        /// </summary>
        [HttpPost("Upload")]
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

        /// <summary>
        /// Erstellt einen neuen Ordner.
        /// </summary>
        [HttpPost("CreateFolder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFolder([FromBody] FileActionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
            var newFolderPath = Path.Combine(userFolderPath, request.FolderName);

            if (string.IsNullOrWhiteSpace(request.FolderName) || request.FolderName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return BadRequest(new { message = "Der Ordnername enthält ungültige Zeichen." });
            }

            if (Directory.Exists(newFolderPath))
            {
                return BadRequest(new { message = "Ein Ordner mit diesem Namen existiert bereits." });
            }

            Directory.CreateDirectory(newFolderPath);
            return Ok(new { message = "Ordner erfolgreich erstellt." });
        }

        /// <summary>
        /// Löscht eine einzelne Datei.
        /// </summary>
        [HttpPost("DeleteFile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFile([FromBody] FileActionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
            var filePath = Path.Combine(userFolderPath, request.FileName);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { message = "Datei erfolgreich gelöscht." });
                }
                return NotFound(new { message = "Datei nicht gefunden." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen der Datei {FileName}", request.FileName);
                return StatusCode(500, new { message = "Die Datei konnte nicht gelöscht werden." });
            }
        }

        /// <summary>
        /// Benennt eine einzelne Datei um.
        /// </summary>
        [HttpPost("RenameFile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenameFile([FromBody] FileActionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
            var oldFilePath = Path.Combine(userFolderPath, request.FileName);
            var newFilePath = Path.Combine(userFolderPath, request.NewFileName);

            try
            {
                if (!System.IO.File.Exists(oldFilePath))
                {
                    return NotFound(new { message = "Originaldatei nicht gefunden." });
                }
                if (System.IO.File.Exists(newFilePath))
                {
                    return BadRequest(new { message = "Eine Datei mit dem neuen Namen existiert bereits." });
                }
                System.IO.File.Move(oldFilePath, newFilePath);
                return Ok(new { message = "Datei erfolgreich umbenannt." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Umbenennen der Datei {FileName}", request.FileName);
                return StatusCode(500, new { message = "Die Datei konnte nicht umbenannt werden." });
            }
        }

        /// <summary>
        /// Löscht einen Ordner (inklusive Inhalt).
        /// </summary>
        [HttpPost("DeleteFolder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFolder([FromBody] FileActionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
            var folderPath = Path.Combine(userFolderPath, request.FolderName);

            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true); // true => rekursives Löschen
                    return Ok(new { message = "Ordner erfolgreich gelöscht." });
                }
                return NotFound(new { message = "Ordner nicht gefunden." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des Ordners {FolderName}", request.FolderName);
                return StatusCode(500, new { message = "Der Ordner konnte nicht gelöscht werden." });
            }
        }

        /// <summary>
        /// Benennt einen Ordner um.
        /// </summary>
        [HttpPost("RenameFolder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenameFolder([FromBody] FileActionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
            var oldFolderPath = Path.Combine(userFolderPath, request.FolderName);
            var newFolderPath = Path.Combine(userFolderPath, request.NewFolderName);

            try
            {
                if (!Directory.Exists(oldFolderPath))
                {
                    return NotFound(new { message = "Originalordner nicht gefunden." });
                }
                if (Directory.Exists(newFolderPath))
                {
                    return BadRequest(new { message = "Ein Ordner mit dem neuen Namen existiert bereits." });
                }
                Directory.Move(oldFolderPath, newFolderPath);
                return Ok(new { message = "Ordner erfolgreich umbenannt." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Umbenennen des Ordners {FolderName}", request.FolderName);
                return StatusCode(500, new { message = "Der Ordner konnte nicht umbenannt werden." });
            }
        }

        private long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path)) return 0;
            return new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }
    }

    /// <summary>
    /// Eine flexible Klasse, um Anfragen für verschiedene Datei- und Ordner-Aktionen zu handhaben.
    /// </summary>
    public class FileActionRequest
    {
        public string? FileName { get; set; }
        public string? NewFileName { get; set; }
        public string? FolderName { get; set; }
        public string? NewFolderName { get; set; }
    }
}