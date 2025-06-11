using System.ComponentModel.DataAnnotations;

namespace MemoriaPiDataCore.Models
{
    public class PasswordResetRequest
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Die ID des Benutzers, der die Anfrage stellt
        public ApplicationUser User { get; set; } // Navigations-Eigenschaft

        public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
        public string? ApprovedByAdminId { get; set; } // ID des Admins, der genehmigt hat
    }
}