using Microsoft.AspNetCore.Identity;

namespace AuthenticationNetCore.Models
{
    public class AppUser:IdentityUser
    {
        public string? FullName { get; set; }
    }

}
