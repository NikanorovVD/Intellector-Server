using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using ServiceLayer.Models;
using Shared.Models;
using ClientErrors;

namespace ServiceLayer.Services
{
    public class UsersService : IUsersService
    {
        private const string _login_fail_message = "Bad Credentials";

        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public UsersService(UserManager<AppUser> userManager, AppDbContext dbContext, IMapper mapper)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<string> CreateAsync(UserPasswordDto user)
        {
            AppUser appUser = new AppUser()
            {
                UserName = user.UserName,
                Roles = user.Roles.Select(r => new UserRole() { RoleId = r.Id }).ToList()
            };

            await ValidatePassword(appUser, user.Password);

            var result = await _userManager.CreateAsync(appUser, user.Password);
            if (!result.Succeeded) throw new Exception(result.ToString());
            return appUser.Id;
        }


        public async Task DeleteAsync(string id)
        {
            AppUser? appUser = await _userManager.FindByIdAsync(id)
                ?? throw new Exception("User not found");
            var result = await _userManager.DeleteAsync(appUser);
            if (!result.Succeeded)
            {
                throw new Exception(result.ToString());
            }
        }

        public async Task<UserDto?> GetAsync(string id, CancellationToken cancellation_token)
        {
            return await _dbContext.Users
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellation_token);
        }

        public async Task<UserDto> GetByCredentials(string userName, string password, CancellationToken cancellation_token)
        {
            AppUser user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpper(), cancellation_token)              
                 ?? throw new ClientError(_login_fail_message);

            if (!await _userManager.CheckPasswordAsync(user, password))
                throw new ClientError(_login_fail_message);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellation_token)
        {
            return await _dbContext.Users
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync(cancellation_token);
        }

        public async Task UpdateAsync(UserPasswordDto user)
        {
            AppUser? appUser = await _dbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == user.Id)
                ?? throw new Exception("User not found");

            if (user.UserName != null)
            {
                appUser.UserName = user.UserName;
            }

            if (user.Password != null)
            {
                await ValidatePassword(appUser, user.Password);
                var passwordHasher = new PasswordHasher<AppUser>();
                appUser.PasswordHash = passwordHasher.HashPassword(appUser, user.Password);
            }

            if (user.Roles != null)
            {
                appUser.Roles = user.Roles.Select(r => new UserRole() { RoleId = r.Id }).ToList();
            }

            var result = await _userManager.UpdateAsync(appUser);
            if (!result.Succeeded) throw new Exception(result.ToString());
        }

        private async Task ValidatePassword(AppUser appUser, string password)
        {
            var passwordValidator = new PasswordValidator<AppUser>();
            var passwordErrors = await passwordValidator.ValidateAsync(_userManager, appUser, password);
            if (!passwordErrors.Succeeded) throw new ClientError(passwordErrors.ToString());
        }
    }
}
