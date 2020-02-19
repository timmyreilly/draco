// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;
using Draco.Azure.Options;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Azure.Execution
{
    public class ServiceBusExecutionRequestSubscriber : IExecutionRequestSubscriber
    {
        private readonly ILogger logger;
        private readonly IExecutionRequestRouter requestRouter;
        private readonly IServiceBusSubscriptionOptions subscriptionOptions;
        private readonly SubscriptionClient subscriptionClient;

        public ServiceBusExecutionRequestSubscriber(
            ILogger<ServiceBusExecutionRequestSubscriber> logger,
            IExecutionRequestRouter requestRouter,
            IOptionsSnapshot<ServiceBusSubscriptionOptions<ServiceBusExecutionRequestSubscriber>> optionsSnapshot)
            : this(logger, requestRouter, optionsSnapshot.Value) { }

        public ServiceBusExecutionRequestSubscriber(
            ILogger logger,
            IExecutionRequestRouter requestRouter, 
            IServiceBusSubscriptionOptions subscriptionOptions)
        {
            this.logger = logger;
            this.requestRouter = requestRouter;
            this.subscriptionOptions = subscriptionOptions;

            this.subscriptionClient = new SubscriptionClient(
                this.subscriptionOptions.ConnectionString,
                this.subscriptionOptions.TopicName,
                this.subscriptionOptions.SubscriptionName);
        }

        public bool IsSubscribed { get; private set; }

        public Task SubscribeAsync(CancellationToken cancelToken)
        {
            if (IsSubscribed == false)
            {
                var handlerOptions = new MessageHandlerOptions(OnMessageHandlerException)
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 1
                };

                subscriptionClient.RegisterMessageHandler(OnMessageReceived, handlerOptions);

                cancelToken.Register(() => subscriptionClient.CloseAsync().Wait());

                logger.LogInformation($"Subscribed to Azure service bus namespace [{subscriptionClient.ServiceBusConnection.Endpoint}] " +
                                      $"subscription [{subscriptionOptions.TopicName} >> {subscriptionOptions.SubscriptionName}]. " +
                                      $"Awaiting execution requests...");

                IsSubscribed = true;
            }

            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(Message message, CancellationToken cancelToken)
        {
            logger.LogInformation($"Processing message [{message.MessageId}]...");

            if (cancelToken.IsCancellationRequested)
            {
                logger.LogWarning($"Cancellation requested. Abandoning message [{message.MessageId}]...");

                await subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
            }
            else
            {
                try
                {
                    var messageJson = Encoding.UTF8.GetString(message.Body);
                    var executionRequest = JsonConvert.DeserializeObject<ExecutionRequest>(messageJson);

                    logger.LogInformation($"Processing execution request [{executionRequest.ExecutionId}]...");

                    await requestRouter.RouteRequestAsync(executionRequest, cancelToken);

                    logger.LogInformation($"Execution request [{executionRequest.ExecutionId}] successfully processed.");

                    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                }
                catch (Exception ex)
                {
                    logger.LogError($"An error occurred while processing message [{message.MessageId}]: [{ex.Message}]. " +
                                    $"Abandoning message [{message.MessageId}]...");

                    await subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
                }
            }
        }

        private Task OnMessageHandlerException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            logger.LogError($"An error occurred while processing a message: \n" +
                            $"Action:        [{exceptionReceivedEventArgs.ExceptionReceivedContext.Action}]; \n" +
                            $"Client Id:     [{exceptionReceivedEventArgs.ExceptionReceivedContext.ClientId}]; \n" +
                            $"Endpoint:      [{exceptionReceivedEventArgs.ExceptionReceivedContext.Endpoint}]; \n" +
                            $"Entity Path:   [{exceptionReceivedEventArgs.ExceptionReceivedContext.EntityPath}]; \n" +
                            $"Error Message: [{exceptionReceivedEventArgs.Exception.Message}].");

            return Task.CompletedTask;
        }
    }
}
