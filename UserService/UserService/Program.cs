using ClientErrors;
using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiceLayer.Configuration;
using ServiceLayer.Services;
using System.Text;
using UserService.Automapper;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // configuration
            builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));

            // services
            services.AddScoped<IUsersService, UsersService>();

            // controllers
            services.AddControllers();

            // swagger
            services.AddOpenApi();
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "User Service",
                    Description = "Сервис для управления пользователями",
                });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            // identity
            services.AddIdentityCore<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

            // database
            services.AddDbContext<AppDbContext>(options
              => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // auth
            services.AddAuthentication("Bearer")
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])
                    )
                });

            // hosted service
            services.AddHostedService<DatabaseInitService>();

            // validation
            // not implemented yet

            // automapper
            services.AddAutoMapper(typeof(AppMappingProfile));

            var app = builder.Build();

            // middleware
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
            app.UseClientErrors();
            app.MapControllers();

            // run
            app.Run();
        }
    }
}
