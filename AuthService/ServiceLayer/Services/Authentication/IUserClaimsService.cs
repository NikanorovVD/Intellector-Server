using Shared.Models;
using System.Security.Claims;

namespace ServiceLayer.Services.Authentication
{
    public interface IUserClaimsService
    {
        public Task<IEnumerable<Claim>> GetUserClaimsAsync(UserDto user, CancellationToken cancellationToken);
    }
}
