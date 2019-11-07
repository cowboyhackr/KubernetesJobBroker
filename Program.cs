using System;
using KubernetesJobBroker;
using KubernetesJobBroker.BrokerServices;
using KubernetesJobBroker.Models;

namespace KubernetesJobBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start ModelSync Broker!");
            var brokerController = new CompositionRoot().RegisterServices();

            brokerController.Start();

            Console.ReadLine();

        }
    }
}
