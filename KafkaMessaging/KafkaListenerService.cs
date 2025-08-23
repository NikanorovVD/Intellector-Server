using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaMessaging
{
    public class KafkaListenerService<TMessage>
    {
        private readonly IConsumer<Null, TMessage> _consumer;
        private readonly string _topic;
        private readonly ILogger<KafkaListenerService<TMessage>> _logger;
        public event Func<TMessage, Task> OnMessageAsync;

        public KafkaListenerService(IOptions<KafkaSettings> options, ILogger<KafkaListenerService<TMessage>> logger)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = options.Value.Consumer.BootstrapServers,
                GroupId = options.Value.Consumer.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _topic = options.Value.Consumer.Topic;
            _consumer = new ConsumerBuilder<Null, TMessage>(config)
                .SetValueDeserializer(new KafkaJsonDeserializer<TMessage>())
                .Build();

            _logger = logger;
        }

        public async void ConsumeMessages(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<Null, TMessage> consumeResult = _consumer.Consume(cancellationToken);
                    await OnMessageAsync?.Invoke(consumeResult.Message.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError("{Error}", ex.ToString());
                }
            }
        }
    }
}
