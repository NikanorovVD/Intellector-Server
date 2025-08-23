using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ServiceLayer.Configuration;
using Shared.Constants;

namespace ServiceLayer.Services
{
    public class DatabaseInitService : BackgroundService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly AdminSettings _adminSettings;

        public DatabaseInitService(IServiceScopeFactory serviceScopeFactory, IOptions<AdminSettings> options)
        {
            var scope = serviceScopeFactory.CreateScope();
            _userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();
            _roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            _dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            _adminSettings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await MigrateDatabaseAsync();
            await SeedDatabaseDataAsync();
        }

        private async Task MigrateDatabaseAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }

        private async Task SeedDatabaseDataAsync()
        {
            foreach (var roleName in AppRoles.AllUserRoles())
            {
                IdentityRole<string>? identity_role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (identity_role == null)
                {
                    IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded) throw new Exception(result.ToString());
                }
            }

            AppUser? adminUser = await _userManager.FindByNameAsync(_adminSettings.UserName);
            if (adminUser == null)
            {
                adminUser = new AppUser()
                {
                    UserName = _adminSettings.UserName,                   
                };

                IdentityResult result = await _userManager.CreateAsync(adminUser, _adminSettings.Password);
                if (!result.Succeeded) throw new Exception(result.ToString());

                result = await _userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
                if (!result.Succeeded) throw new Exception(result.ToString());
            }
        }
    }
}
