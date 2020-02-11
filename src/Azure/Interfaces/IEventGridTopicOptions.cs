namespace Draco.Azure.Interfaces
{
    public interface IEventGridTopicOptions
    {
        string TopicEndpoint { get; }
        string TopicKey { get; }
    }
}
