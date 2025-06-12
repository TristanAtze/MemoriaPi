using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MemoriaPiWeb.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Benutzername")]
        public string UserName { get; set; }

        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Display(Name = "Aktuelles Profilbild")]
        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "Neues Profilbild hochladen")]
        public IFormFile? NewProfilePicture { get; set; }

        [Display(Name = "Muss Passwort ändern")]
        public bool MustChangePassword { get; set; }

        [Display(Name = "Account gesperrt")]
        public bool IsLockedOut { get; set; }

        public IList<string> CurrentRoles { get; set; } = new List<string>();

        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();

        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}