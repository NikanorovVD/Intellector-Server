namespace Shared.Models
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IEnumerable<RoleDto> Roles { get; set; }
    }
}
