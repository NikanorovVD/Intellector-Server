using DataLayer;
using GameService.Automapper;
using GenericCrud;
using KafkaMessaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ServiceLayer.Configuration;
using ServiceLayer.Hubs;
using ServiceLayer.Services;
using Shared.Models;
using StackExchange.Redis;


namespace GameService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<GameRedisService>();
            builder.Services.AddSignalR();

            builder.Services.AddOpenApi();
            builder.Services.AddAppSwagger();
            builder.Services.AddAutoMapper(typeof(AppMappingProfile));

            builder.Services.AddAppAuthentication(builder.Configuration);

            // Redis
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
                return ConnectionMultiplexer.Connect(settings.ConnectionString);
            });

            // Kafka
            builder.Services.AddKafka(builder.Configuration.GetSection("Kafka"));
            builder.Services.AddKafkaProducer<MoveDto>();
            builder.Services.AddKafkaConsumer<GameStartKafkaConsumerService, CreateGameRequest>();


            var app = builder.Build();


            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
            app.MapHub<GameHub>("/game");
            app.Run();
        }
    }
}
