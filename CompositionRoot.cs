using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KubernetesJobBroker.Models;
using KubernetesJobBroker.BrokerServices;

namespace KubernetesJobBroker
{
    public class CompositionRoot
    {
        private IConfiguration configuration;

        public CompositionRoot()
        {
            BuildConfig();
            RegisterServices();
        }

        private void BuildConfig(){
                   // Set up configuration sources.
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("secrets/appsettings.secrets.json", optional: true);


        configuration = builder.Build();

        }
        public BrokerController RegisterServices()
        {
            var services = new ServiceCollection();
            var KubernetesJob = configuration.GetSection("KubernetesJob").Get<KubernetesJob>();
            var serviceBusConfig = configuration.GetSection("ServiceBus").Get<ServiceBusConfig>();
            //services.AddOptions();
            services.AddSingleton(KubernetesJob);
            
            services.AddSingleton(serviceBusConfig);

            
            services.AddSingleton<IK8sJobService, K8sJobService>();
            services.AddSingleton<IQueueService, QueueService>();
            services.AddSingleton<BrokerController>();
            var serviceProvider = services.BuildServiceProvider();
            var brokerController = serviceProvider.GetService<BrokerController>();

            return brokerController;
        }
    }
}