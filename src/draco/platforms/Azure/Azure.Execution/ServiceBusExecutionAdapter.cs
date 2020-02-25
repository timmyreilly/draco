// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;
using Draco.Azure.Options;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Azure.Execution
{
    public class ServiceBusExecutionAdapter : IAsyncExecutionDispatcher, IExecutionAdapter
    {
        private readonly TopicClient topicClient;

        public ServiceBusExecutionAdapter(
            IOptionsSnapshot<ServiceBusTopicOptions<ServiceBusExecutionAdapter>> optionsSnapshot)
            : this(optionsSnapshot.Value) { }

        public ServiceBusExecutionAdapter(IServiceBusTopicOptions topicOptions)
        {
            this.topicClient = new TopicClient(topicOptions.ConnectionString, topicOptions.TopicName);
        }

        public async Task<Core.Models.ExecutionContext> ExecuteAsync(ExecutionRequest request, CancellationToken cancelToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestJson = JsonConvert.SerializeObject(request);
            var requestMessage = new Message(Encoding.UTF8.GetBytes(requestJson));

            requestMessage.UserProperties.Add(nameof(ExecutionRequest.ExecutionModelName), request.ExecutionModelName);
            requestMessage.UserProperties.Add(nameof(ExecutionRequest.ExecutionProfileName), request.ExecutionProfileName);
            requestMessage.UserProperties.Add(nameof(ExecutionRequest.Priority), request.Priority.ToString());

            await topicClient.SendAsync(requestMessage);

            return request.ToExecutionContext().UpdateStatus(ExecutionStatus.Queued);
        }
    }
}
