using AuthenticationNetCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationNetCore.Data
{
    public class AppDBContext:IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
        {

        }
    }
}
