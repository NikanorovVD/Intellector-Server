using Shared.Models;

namespace ServiceLayer.Services.Authentication
{
    public interface ISignInService
    {
        public Task<UserDto> SignUserAsync(string username, string password, CancellationToken cancellationToken);
    }
}
