using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using GenericCrud;
using KafkaMessaging;
using MatchMaking.Automapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceLayer.Models;
using ServiceLayer.Services;
using Shared.Models;

namespace PlantWatch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddScoped<MatchingService>();

            builder.Services.AddOpenApi();
            builder.Services.AddAppSwagger();
            builder.Services.AddAutoMapper(typeof(AppMappingProfile));

            builder.Services.AddAppAuthentication(builder.Configuration);

            builder.Services.AddDbContext<AppDbContext>(options
               => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddGenericCrud<AppDbContext>();
            builder.Services.AddHostedService<DatabaseInitService>();

            builder.Services.AddKafka(builder.Configuration.GetSection("Kafka"));
            builder.Services.AddKafkaProducer<CreateGameRequest>();

            var app = builder.Build();

            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
