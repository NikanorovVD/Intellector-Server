using Microsoft.AspNetCore.Identity;

namespace DataLayer.Entities
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual IdentityRole Role { get; set; }
    }
}
