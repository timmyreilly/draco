using System;

namespace Draco.Core.Execution.Interfaces
{
    public interface IInputObjectOptions
    {
        TimeSpan DefaultTimeoutDuration { get; }
    }
}
