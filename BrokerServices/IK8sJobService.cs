using k8s.Models;

namespace KubernetesJobBroker.BrokerServices
{
    public interface IK8sJobService
    {
        V1JobStatus CreateK8sJob(dynamic messageBody);
    }
}