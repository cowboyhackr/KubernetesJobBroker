using System;
using k8s;
using k8s.Models;
using System.Collections.Generic;
using KubernetesJobBroker.Models;
using System.Text.Json;

#region KubeNotes
// KUBE_TOKEN=$(cat /var/run/secrets/kubernetes.io/serviceaccount/token)
// curl -sSk -H "Authorization: Bearer $KUBE_TOKEN" \
//       https://$KUBERNETES_SERVICE_HOST:$KUBERNETES_PORT_443_TCP_PORT/api/v1/namespaces/default/pods/$HOSTNAME

// $ kubectl proxy
// $ curl -X POST -H 'Content-Type: application/yaml' --data '
// apiVersion: batch/v1
// kind: Job
// metadata:
//   name: example-job
// spec:
//   template:
//     metadata:
//       name: example-job
//     spec:
//       containers:
//       - name: pi
//         image: perl
//         command: ["perl",  "-Mbignum=bpi", "-wle", "print bpi(2000)"]
//       restartPolicy: Never
// ' http://127.0.0.1:8001/apis/batch/v1/namespaces/default/jobs

#endregion

namespace KubernetesJobBroker.BrokerServices
{
    public class K8sJobService : IK8sJobService
    {
        private readonly KubernetesJob kubernetesJob;

        public K8sJobService(KubernetesJob kubernetesJob) 
        {
            this.kubernetesJob = kubernetesJob;
        }

        public V1JobStatus CreateK8sJob(dynamic messageBody)
        {

            var job = new V1Job();

            var container = new V1Container();
            container.Image = kubernetesJob.ImageName;
            container.Name = kubernetesJob.KubernetesNamespace;
            container.Command = new List<string>();

            // container entry point
            container.Command.Add("dotnet");
            container.Command.Add(kubernetesJob.EntryPoint);

            // job arguments
            container.Command.Add(messageBody);

            job.Kind = "Job";
            job.ApiVersion = "batch/v1";
            job.Metadata = new V1ObjectMeta();
            job.Metadata.Name = $"{kubernetesJob.JobName.ToLower()}-{DateTime.Now.Ticks.ToString()}";
            job.Metadata.NamespaceProperty = kubernetesJob.KubernetesNamespace;
            job.Spec = new V1JobSpec();
            job.Spec.Template = new V1PodTemplateSpec();
            job.Spec.Template.Spec = new V1PodSpec();
            job.Spec.Template.Spec.Containers = new List<V1Container>();
            job.Spec.Template.Spec.Containers.Add(container);
            job.Spec.Template.Spec.RestartPolicy = "Never";
            job.Validate();

            var config = KubernetesClientConfiguration.BuildDefaultConfig();
            Console.WriteLine(config);
            IKubernetes client = new Kubernetes(config);
            var createdJob = client.CreateNamespacedJob(job, kubernetesJob.KubernetesNamespace);
            return createdJob.Status;
        }
    }
}