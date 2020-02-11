namespace Draco.Azure.Interfaces
{
    public interface IServiceBusTopicOptions
    {
        string ConnectionString { get; }
        string TopicName { get; }
    }
}
