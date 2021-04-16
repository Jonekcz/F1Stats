using Microsoft.AspNetCore.Identity;

namespace F1_Stats.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
