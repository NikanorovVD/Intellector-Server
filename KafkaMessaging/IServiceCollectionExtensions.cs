using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaMessaging
{
    public static class IServiceCollectionExtensions
    {
        public static void AddKafka(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.AddSingleton(typeof(KafkaMessageService<>), typeof(KafkaMessageService<>));
            services.AddSingleton(typeof(KafkaListenerService<>), typeof(KafkaListenerService<>));
            services.Configure<KafkaSettings>(configurationSection);
        }

        public static void AddKafkaConsumer<TConsumer, TMessage>(this IServiceCollection services) where TConsumer: KafkaConsumerBackgroundService<TMessage>
        {
            services.AddHostedService<TConsumer>();
        }

        public static void AddKafkaProducer<TMessage>(this IServiceCollection services)
        {
            services.AddSingleton<KafkaProducerBackgroundService<TMessage>>();
            services.AddHostedService<KafkaProducerBackgroundService<TMessage>>(provider => provider.GetService<KafkaProducerBackgroundService<TMessage>>()!);
        }
    }
}
