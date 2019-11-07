namespace KubernetesJobBroker.Models
{
    public class ServiceBusConfig
    {
        public string ConnectionString { get; set; }
        public string ExportQueueName { get; set; }
        public string BlobToNoSqlQueueName { get; set; }
    }
}