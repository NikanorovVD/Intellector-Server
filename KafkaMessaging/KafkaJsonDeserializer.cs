using Confluent.Kafka;
using System.Runtime.Serialization;
using System.Text.Json;


namespace KafkaMessaging
{
    internal class KafkaJsonDeserializer<TMessage> : IDeserializer<TMessage>
    {
        public TMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<TMessage>(data)
                ?? throw new SerializationException("Error deserializing kafka response data");
        }
    }
}
