using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ServiceLayer.Configuration;
using ServiceLayer.Models;
using ServiceLayer.Services.Authentication;
using Shared.Constants;
using Shared.Models;


namespace ServiceLayer.Services
{
    public class TokenUpdateService : BackgroundService
    {
        private readonly ITokenService _tokenService;
        private readonly TimeSpan _expirationTime;
        private readonly TokenStorage _tokenStorage;

        public TokenUpdateService(IServiceScopeFactory serviceScopeFactory, IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
        {
            var scope = serviceScopeFactory.CreateScope();
            _tokenService = scope.ServiceProvider.GetService<ITokenService>();
            _tokenStorage = scope.ServiceProvider.GetService<TokenStorage>();
            _expirationTime = new TimeSpan(0, minutes: jwtSettings.Value.ExperationTimeMinutes, 0);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await UpdateTokenAsync(stoppingToken);
            await Task.Delay(_expirationTime / 2, stoppingToken);
        }

        private async Task UpdateTokenAsync(CancellationToken stoppingToken)
        {
            UserDto serviceUser = new()
            {
                Id = string.Empty,
                UserName = "Internal auth service",
                Roles = [new RoleDto(string.Empty, AppRoles.AuthService)]
            };

            TokenDto tokenDto = await _tokenService.CreateTokenAsync(serviceUser, stoppingToken);
            string token = tokenDto.Token;

            _tokenStorage.UpdateToken(token);
        }
    }
}
