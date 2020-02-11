using Draco.Azure.Interfaces;

namespace Draco.Azure.Options
{
    public class EventGridTopicOptions : IEventGridTopicOptions
    {
        public string TopicEndpoint { get; set; }
        public string TopicKey { get; set; }
    }

    public class EventGridTopicOptions<T> : EventGridTopicOptions { }
}
