namespace KafkaMessaging
{
    public class KafkaSettings
    {
        public KafkaProducerConfig Producer { get; set; }
        public KafkaConsumerConfig Consumer { get; set; }
    }

    public class KafkaProducerConfig
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
    }

    public class KafkaConsumerConfig
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public string Topic { get; set; }
    }
}
