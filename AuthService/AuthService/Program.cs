using ServiceLayer.Configuration;
using ServiceLayer.Services.Authentication;
using ServiceLayer.Services.Authentication.Concrete;
using Microsoft.OpenApi.Models;
using ServiceLayer.Services;
using ClientErrors;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // configuration
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<UrlsSettings>(builder.Configuration.GetSection("Services"));

            // services
            services.AddScoped<ISignInService, SignInService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IUserClaimsService, UserClaimsService>();
            services.AddSingleton<TokenStorage>();

            // controllers
            services.AddControllers();

            // http client
            services.AddHttpClient();

            // swagger
            services.AddOpenApi();
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Auth Service",
                    Description = "Сервис для выдачи JWT-токенов",
                });
            });

            // hosted service
            services.AddHostedService<TokenUpdateService>();

            var app = builder.Build();

            // middleware
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseClientErrors();
            app.MapControllers();

            // run
            app.Run();
        }
    }
}
