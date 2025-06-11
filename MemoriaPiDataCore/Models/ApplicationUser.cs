using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser() { }
    public bool HasAccess { get; set; } = false;
    public string? ProfilePictureUrl { get; set; }

    public bool MustChangePassword { get; set; } = false;
}