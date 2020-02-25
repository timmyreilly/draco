// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;

namespace Draco.Azure.Options
{
    public class ServiceBusTopicOptions : IServiceBusTopicOptions
    {
        public string ConnectionString { get; set; }
        public string TopicName { get; set; }
    }

    public class ServiceBusTopicOptions<T> : ServiceBusTopicOptions { }
}
