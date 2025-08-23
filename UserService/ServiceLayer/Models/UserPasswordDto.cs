using Shared.Models;

namespace ServiceLayer.Models
{
    public class UserPasswordDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public IEnumerable<RoleDto> Roles { get; set; }
    }
}
