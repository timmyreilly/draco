using Draco.Azure.Interfaces;
using Draco.Azure.Options;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Models.Extensions;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draco.Azure.Execution
{
    public class EventGridExecutionUpdatePublisher : IExecutionUpdatePublisher
    {
        public const string EventType       = "Core.Models.ExecutionUpdateEvent";
        public const string EventVersion    = "1.0";

        private readonly EventGridClient eventGridClient;
        private readonly string topicHostName;

        public EventGridExecutionUpdatePublisher(
            IOptionsSnapshot<EventGridTopicOptions<EventGridExecutionUpdatePublisher>> optionsSnapshot)
            : this(optionsSnapshot.Value) { }

        public EventGridExecutionUpdatePublisher(IEventGridTopicOptions topicOptions)
        {
            var topicCredentials = new TopicCredentials(topicOptions.TopicKey);

            this.eventGridClient = new EventGridClient(topicCredentials);
            this.topicHostName = new Uri(topicOptions.TopicEndpoint).Host;
        }

        public async Task PublishUpdateAsync(Core.Models.Execution execution)
        {
            if (execution == null)
            {
                throw new ArgumentNullException(nameof(execution));
            }

            await eventGridClient.PublishEventsAsync(topicHostName, new List<EventGridEvent>
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = EventType,
                    Data = execution.ToEvent(),
                    EventTime = DateTime.UtcNow,
                    Subject = execution.ExecutionId,
                    DataVersion = EventVersion
                }
            });
       }
    }
}
