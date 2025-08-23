using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaMessaging
{
    public abstract class KafkaConsumerBackgroundService<TMessage> : BackgroundService
    {
        private readonly KafkaListenerService<TMessage> _kafkaListenerService;

        protected KafkaConsumerBackgroundService(KafkaListenerService<TMessage> kafkaListenerService)
        {
            _kafkaListenerService = kafkaListenerService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _kafkaListenerService.OnMessageAsync += ProcessMessageAsync;
            Task.Run(() => _kafkaListenerService.ConsumeMessages(stoppingToken), stoppingToken);
            return Task.CompletedTask;
        }

        protected abstract Task ProcessMessageAsync(TMessage response);
    }
}
