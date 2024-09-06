using Microsoft.AspNetCore.Identity;

namespace gadget_mania.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AppUser class
public class AppUser : IdentityUser
{
    [PersonalData]
    public required string FirstName { get; set; }
    [PersonalData]
    public required string LastName { get; set; }

    [PersonalData]
    public string ProfilePicture { get; set; } = string.Empty;
}

