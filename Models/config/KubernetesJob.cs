namespace KubernetesJobBroker.Models
{
    public class KubernetesJob
    {
        public string ImageName { get; set; }
        public string KubernetesNamespace { get; set; }
        public string JobName { get; set; }
        public string EntryPoint { get; set; }
    }
}