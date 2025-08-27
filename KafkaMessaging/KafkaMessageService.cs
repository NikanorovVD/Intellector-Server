using Confluent.Kafka;

namespace KafkaMessaging
{
    public class KafkaMessageService<TMessage>
    {
        private readonly IProducer<string, TMessage> _producer;
        private readonly string _topic;

        public KafkaMessageService(KafkaProducerConfig options)
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = options.BootstrapServers
            };

            _producer = new ProducerBuilder<string, TMessage>(config)
                .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
                .Build();

            _topic = options.Topic;
        }

        public async Task SendMessageAsync(TMessage message)
        {
            await _producer.ProduceAsync(_topic, new Message<string, TMessage>()
            {
                Value = message
            });
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
