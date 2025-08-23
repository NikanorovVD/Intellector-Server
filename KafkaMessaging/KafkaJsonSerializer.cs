using Confluent.Kafka;
using System.Text.Json;

namespace KafkaMessaging
{
    public class KafkaJsonSerializer<TMessage> : ISerializer<TMessage>
    {
        public byte[] Serialize(TMessage data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }
    }
}
