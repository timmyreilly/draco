// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Azure.Interfaces
{
    public interface IServiceBusTopicOptions
    {
        string ConnectionString { get; }
        string TopicName { get; }
    }
}
