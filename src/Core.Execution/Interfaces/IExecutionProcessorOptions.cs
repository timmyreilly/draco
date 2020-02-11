using System;

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionProcessorOptions
    {
        TimeSpan DefaultExecutionTimeoutDuration { get; }
    }
}
