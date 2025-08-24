using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using AuthService.Models;
using ServiceLayer.Configuration;
using Shared.Models;

namespace ServiceLayer.Services.Authentication.Concrete
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ISignInService _signInService;
        private readonly IUserClaimsService _userClaimsService;

        public JwtTokenService(IOptions<JwtSettings> options, ISignInService signInService, IUserClaimsService userClaimsService)
        {
            _jwtSettings = options.Value;
            _signInService = signInService;
            _userClaimsService = userClaimsService;
        }

        public async Task<TokenDto> CreateTokenAsync(string username, string password, CancellationToken cancellationToken)
        {
            UserDto user = await _signInService.SignUserAsync(username, password, cancellationToken);
            return await CreateTokenAsync(user, cancellationToken);           
        }

        public async Task<TokenDto> CreateTokenAsync(UserDto user, CancellationToken cancellationToken)
        {
            DateTime expiration = DateTime.Now.AddMinutes(_jwtSettings.ExperationTimeMinutes);

            IEnumerable<Claim> claims = await _userClaimsService.GetUserClaimsAsync(user, cancellationToken);

            SigningCredentials credentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.Key)
                ),
                SecurityAlgorithms.HmacSha256
            );

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: credentials,
                expires: expiration
                );

            string token_value = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenDto
            {
                Token = token_value,
                Expiration = expiration
            };
        }
    }
}
