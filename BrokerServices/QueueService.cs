using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using KubernetesJobBroker.Models;
using System.Linq;

namespace KubernetesJobBroker.BrokerServices
{
    public class QueueService : IQueueService
    {
        private readonly IQueueClient queueClient;
        private readonly IK8sJobService k8SJobService;

        public QueueService(ServiceBusConfig serviceBusConfig, IK8sJobService k8sJobService)
        {
            this.queueClient = new QueueClient(serviceBusConfig.ConnectionString,
                                            serviceBusConfig.ExportQueueName);
            k8SJobService = k8sJobService;

            RegisterOnMessageHandlerAndReceiveMessages();
        }

        void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                var workflowJobId = message.UserProperties.SingleOrDefault(x => x.Key == "WorkflowJobId");
                var messageBody = Encoding.UTF8.GetString(message.Body);

                // TODO Log Message
                Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

                k8SJobService.CreateK8sJob(messageBody);
                // Complete the message so that it is not received again.
                // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
                await queueClient.CompleteAsync(message.SystemProperties.LockToken);

                // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
                // If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
                // to avoid unnecessary exceptions.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Handle Message processing exception!");

                Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            }
        }

        Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}