using Shared.Models;
using System.Security.Claims;

namespace ServiceLayer.Services.Authentication.Concrete
{
    public class UserClaimsService : IUserClaimsService
    {
        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(UserDto user, CancellationToken cancellationToken)
        {
            List<Claim> claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                ..user.Roles.Select(userRole => new Claim(ClaimTypes.Role, userRole.Name))
                ];
            return claims;
        }
    }
}
