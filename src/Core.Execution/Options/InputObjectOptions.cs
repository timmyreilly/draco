using Draco.Core.Execution.Interfaces;
using System;

namespace Draco.Core.Execution.Options
{
    public class InputObjectOptions : IInputObjectOptions
    {
        public TimeSpan DefaultTimeoutDuration { get; set; } = TimeSpan.FromHours(1);
    }

    public class InputObjectOptions<T> : InputObjectOptions { }
}
