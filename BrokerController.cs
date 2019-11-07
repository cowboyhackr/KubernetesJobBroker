using KubernetesJobBroker.BrokerServices;

namespace KubernetesJobBroker
{
    public class BrokerController
    {
        private readonly IQueueService queueService;

        public BrokerController(IQueueService queueService)
        {
            this.queueService = queueService;
        }

        public void Start(){

        }
    }
}