using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Models
{
    public class UpdateUserRequest
    {
        public string Id { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }
}
