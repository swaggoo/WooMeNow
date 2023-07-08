using Microsoft.AspNetCore.Identity;

namespace WooMeNow.API.Models;

public class Role : IdentityRole<int>
{
    public ICollection<UserRole> UserRoles { get; set; }
}   
