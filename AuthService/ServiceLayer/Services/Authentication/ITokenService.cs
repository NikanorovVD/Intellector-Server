using ServiceLayer.Models;
using Shared.Models;

namespace ServiceLayer.Services.Authentication
{
    public interface ITokenService
    {
        public Task<TokenDto> CreateTokenAsync(string username, string password, CancellationToken cancellationToken);
        public Task<TokenDto> CreateTokenAsync(UserDto user, CancellationToken cancellationToken);
    }
}
