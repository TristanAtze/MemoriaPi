using MemoriaPiDataCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

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

        // Die Liste enthält jetzt einen allgemeineren Typ, der Dateien und Ordner sein kann
        public List<CloudItemViewModel> CloudItems { get; set; } = new List<CloudItemViewModel>();

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

            // Speicherberechnung
            long usedBytes = GetDirectorySize(userFolderPath);
            long totalBytes = (long)user.StorageCapacityGB * 1024 * 1024 * 1024;
            TotalStorageGB = user.StorageCapacityGB;
            UsedStorageGB = Math.Round((double)usedBytes / (1024 * 1024 * 1024), 2);
            UsedPercentage = totalBytes > 0 ? (int)((double)usedBytes / totalBytes * 100) : 100;

            var directoryInfo = new DirectoryInfo(userFolderPath);

            // Zuerst die Ordner auslesen
            foreach (var dirInfo in directoryInfo.GetDirectories())
            {
                CloudItems.Add(new CloudItemViewModel
                {
                    Name = dirInfo.Name,
                    IsFolder = true,
                    IconClass = "fas fa-folder text-primary" // Festes Icon für Ordner
                });
            }

            // Danach die Dateien auslesen
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                CloudItems.Add(new CloudItemViewModel
                {
                    Name = fileInfo.Name,
                    IsFolder = false,
                    FilePath = $"/uploads/{user.Id}/{fileInfo.Name}",
                    FileSize = FormatFileSize(fileInfo.Length),
                    IconClass = GetIconForFile(fileInfo.Name)
                });
            }

            // Sortieren: Ordner zuerst, dann alphabetisch
            CloudItems = CloudItems.OrderByDescending(item => item.IsFolder).ThenBy(item => item.Name).ToList();
        }

        private long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path)) return 0;
            return new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }

        private string FormatFileSize(long bytes)
        {
            const int unit = 1024;
            if (bytes < unit) return $"{bytes} B";
            var exp = (int)(Math.Log(bytes) / Math.Log(unit));
            return $"{bytes / Math.Pow(unit, exp):F2} {("KMGTPE")[exp - 1]}B";
        }

        private string GetIconForFile(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "fa-file-pdf text-danger",
                ".jpg" or ".jpeg" or ".png" or ".gif" => "fa-file-image text-success",
                ".zip" or ".rar" or ".7z" => "fa-file-archive text-warning",
                ".docx" or ".doc" => "fa-file-word text-info",
                ".xlsx" or ".xls" => "fa-file-excel text-success",
                ".pptx" or ".ppt" => "fa-file-powerpoint text-danger",
                ".txt" => "fa-file-alt text-secondary",
                ".json" => "fa-file-code text-primary",
                _ => "fa-file text-secondary",
            };
        }
    }

    public class CloudItemViewModel
    {
        public string Name { get; set; }
        public bool IsFolder { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
        public string IconClass { get; set; }
    }
}