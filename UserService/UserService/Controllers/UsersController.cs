using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Models;
using ServiceLayer.Services;
using Shared.Constants;
using Shared.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUsersService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(string id, CancellationToken cancellationToken)
        {
            UserDto? user = await _userService.GetAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return user;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<UserDto> users = await _userService.GetAllAsync(cancellationToken);
            return users;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            UserPasswordDto userDto = _mapper.Map<UserPasswordDto>(request);
            string created_id = await _userService.CreateAsync(userDto);
            UserDto? createdUser = await _userService.GetAsync(created_id, cancellationToken);
            return createdUser;
        }

        [HttpPatch]
        public async Task<ActionResult<UserDto>> Update([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            UserPasswordDto userDto = _mapper.Map<UserPasswordDto>(request);

            UserDto? userToUpdate = await _userService.GetAsync(request.Id, cancellationToken);
            if (userToUpdate == null) return NotFound();

            await _userService.UpdateAsync(userDto);
            UserDto? updatedUser = await _userService.GetAsync(request.Id, cancellationToken);
            return updatedUser;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await _userService.GetAsync(id, CancellationToken.None) == null)
                return NotFound();
            await _userService.DeleteAsync(id);
            return Ok();
        }
    }
}
