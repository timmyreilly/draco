// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;

namespace Draco.Azure.Options
{
    public class ServiceBusSubscriptionOptions : IServiceBusSubscriptionOptions
    {
        public string ConnectionString { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
    }

    public class ServiceBusSubscriptionOptions<T> : ServiceBusSubscriptionOptions { }
}
