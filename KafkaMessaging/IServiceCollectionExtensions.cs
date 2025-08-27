using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace KafkaMessaging
{
    public static class IServiceCollectionExtensions
    {
        public static void AddKafka(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.AddSingleton(typeof(KafkaMessageService<>), typeof(KafkaMessageService<>));
            services.AddSingleton(typeof(KafkaListenerService<>), typeof(KafkaListenerService<>));
        }

        public static void AddKafkaConsumer<TConsumer, TMessage>(
            this IServiceCollection services,
            IConfigurationSection configurationSection)
            where TConsumer : KafkaConsumerBackgroundService<TMessage>
        {
            KafkaConsumerConfig settings = configurationSection.Get<KafkaConsumerConfig>();
            services.AddHostedService((serviceProvider) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<KafkaListenerService<TMessage>>>();
                var listenerService = new KafkaListenerService<TMessage>(settings, logger);

                return ActivatorUtilities.CreateInstance<TConsumer>(serviceProvider, listenerService);
            });
        }

        public static void AddKafkaProducer<TMessage>(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            KafkaProducerConfig settings = configurationSection.Get<KafkaProducerConfig>();
            services.AddSingleton((serviceProvider) =>
            {
                var messageService = new KafkaMessageService<TMessage>(settings);
                return ActivatorUtilities.CreateInstance<KafkaProducerBackgroundService<TMessage>>(serviceProvider);
            });
            services.AddHostedService<KafkaProducerBackgroundService<TMessage>>(provider => provider.GetService<KafkaProducerBackgroundService<TMessage>>()!);
        }
    }
}
