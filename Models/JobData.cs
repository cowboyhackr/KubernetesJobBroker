using System;

namespace KubernetesJobBroker.Models
{
    public class JobData
    {
        public string JobId {get;set;}
        public string ModelId { get; set; }
        public string ModelVersionId { get; set; }
        public string JobWorkflowId {get;set;}
        public DateTime BrokerDateTime {get;set;}
    }
}