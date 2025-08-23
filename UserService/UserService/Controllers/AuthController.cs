using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services;
using Shared.Constants;
using Shared.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = AppRoles.AuthService)]
    public class AuthController : ControllerBase
    {
        private readonly IUsersService _userService;

        public AuthController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Auth([FromBody] AuthRequest credentials, CancellationToken cancellationToken)
        {
                return await _userService.GetByCredentials(credentials.UserName, credentials.Password, cancellationToken);
        }
    }
}
