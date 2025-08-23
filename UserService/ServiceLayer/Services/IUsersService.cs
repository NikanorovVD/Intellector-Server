using ServiceLayer.Models;
using Shared.Models;

namespace ServiceLayer.Services
{
    public interface IUsersService
    {
        public Task<UserDto?> GetAsync(string id, CancellationToken cancellation_token);
        public Task<UserDto> GetByCredentials(string userName, string password, CancellationToken cancellation_token);
        public Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellation_token);
        public Task<string> CreateAsync(UserPasswordDto user);
        public Task DeleteAsync(string id);
        public Task UpdateAsync(UserPasswordDto user);
    }
}
