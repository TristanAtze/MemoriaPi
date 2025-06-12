using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MemoriaPiWeb.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Benutzername")]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "Neues Profilbild")]
        public IFormFile? NewProfilePicture { get; set; }

        [Display(Name = "Passwortänderung erzwingen")]
        public bool MustChangePassword { get; set; }

        [Display(Name = "Account gesperrt")]
        public bool IsLockedOut { get; set; }

        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();

        public List<string> SelectedRoles { get; set; } = new List<string>();

        public IEnumerable<string> CurrentRoles { get; set; } = new List<string>();

        [Required]
        [Display(Name = "Speicherplatz (GB)")]
        [Range(1, 2500, ErrorMessage = "Der Wert muss zwischen 1 und 1000 GB liegen.")]
        public int StorageCapacityGB { get; set; }
    }
}