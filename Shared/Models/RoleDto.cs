namespace Shared.Models
{
    public class RoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public RoleDto() { }
        public RoleDto(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
