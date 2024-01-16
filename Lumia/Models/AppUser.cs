using Microsoft.AspNetCore.Identity;

namespace Lumia.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
