using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser() { }
    public bool HasAccess { get; set; } = false;
    public string? ProfilePictureUrl { get; set; }
    public bool MustChangePassword { get; set; } = false;
    public int StorageCapacityGB { get; set; } = 5; 

}