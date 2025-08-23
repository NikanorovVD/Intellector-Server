using Microsoft.AspNetCore.Identity;

namespace DataLayer.Entities
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}
