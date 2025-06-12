using MemoriaPiDataCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MemoriaPiWeb.Pages
{
    [Authorize]
    public class CloudModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        // Eigenschaften für die Speicherberechnung
        public double UsedStorageGB { get; set; }
        public int TotalStorageGB { get; set; }
        public int UsedPercentage { get; set; }

        // NEU: Eine Liste für die Dateien, die angezeigt werden sollen
        public List<FileInfoViewModel> UserFiles { get; set; } = new List<FileInfoViewModel>();

        public CloudModel(UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            var userFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.Id);
            Directory.CreateDirectory(userFolderPath); // Sicherstellen, dass der Ordner existiert

            // Speicherberechnung (wie bisher)
            long usedBytes = GetDirectorySize(userFolderPath);
            long totalBytes = (long)user.StorageCapacityGB * 1024 * 1024 * 1024;
            TotalStorageGB = user.StorageCapacityGB;
            UsedStorageGB = Math.Round((double)usedBytes / (1024 * 1024 * 1024), 2);
            UsedPercentage = totalBytes > 0 ? (int)((double)usedBytes / totalBytes * 100) : 100;

            // NEU: Dateien aus dem Ordner auslesen und zur Liste hinzufügen
            var directoryInfo = new DirectoryInfo(userFolderPath);
            var foundFiles = directoryInfo.GetFiles();

            foreach (var fileInfo in foundFiles)
            {
                UserFiles.Add(new FileInfoViewModel
                {
                    FileName = fileInfo.Name,
                    FilePath = $"/uploads/{user.Id}/{fileInfo.Name}", // Pfad für den Download-Link
                    FileSize = FormatFileSize(fileInfo.Length)
                });
            }
        }

        private long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path)) return 0;
            return new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }

        private string FormatFileSize(long bytes)
        {
            var unit = 1024;
            if (bytes < unit) return $"{bytes} B";
            var exp = (int)(Math.Log(bytes) / Math.Log(unit));
            return $"{bytes / Math.Pow(unit, exp):F2} {("KMGTPE")[exp - 1]}B";
        }
    }

    // NEU: Eine kleine Klasse, um Datei-Informationen an die Seite zu übergeben
    public class FileInfoViewModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
    }
}