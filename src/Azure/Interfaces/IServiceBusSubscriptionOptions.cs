namespace Draco.Azure.Interfaces
{
    public interface IServiceBusSubscriptionOptions
    {
        string ConnectionString { get; }
        string TopicName { get; }
        string SubscriptionName { get; }
    }
}
