using AuthService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Authentication;
using Shared.Models;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public LoginController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<ActionResult<TokenDto>> Login(AuthRequest request, CancellationToken cancellationToken)
        {
            return await _tokenService.CreateTokenAsync(request.UserName, request.Password, cancellationToken);
        }
    }
}
