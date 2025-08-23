using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KafkaMessaging
{
    public class KafkaProducerBackgroundService<TMessage> : BackgroundService
    {
        private readonly KafkaMessageService<TMessage> _kafkaProducer;
        private readonly ConcurrentQueue<TMessage> _messageQueue = [];
        private readonly SemaphoreSlim _semaphore = new(0);
        private readonly ILogger<KafkaProducerBackgroundService<TMessage>> _logger;

        public KafkaProducerBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<KafkaProducerBackgroundService<TMessage>> logger)
        {
            var scope = serviceScopeFactory.CreateScope();
            _kafkaProducer = scope.ServiceProvider.GetService<KafkaMessageService<TMessage>>();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _semaphore.WaitAsync(stoppingToken);
                    while (!_messageQueue.IsEmpty && !stoppingToken.IsCancellationRequested)
                    {
                        if (_messageQueue.TryDequeue(out var message))
                        {
                            await SendMessageToKafka(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("{Error}", ex.ToString());
                }
            }
        }

        private async Task SendMessageToKafka(TMessage message)
        {
            await _kafkaProducer.SendMessageAsync(message);
        }

        public void EnqueueMessage(TMessage message)
        {
            _messageQueue.Enqueue(message);
            _semaphore.Release();
        }
    }
}