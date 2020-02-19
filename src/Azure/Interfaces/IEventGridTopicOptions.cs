// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Azure.Interfaces
{
    public interface IEventGridTopicOptions
    {
        string TopicEndpoint { get; }
        string TopicKey { get; }
    }
}
