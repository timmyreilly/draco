using Draco.Core.Execution.Interfaces;
using System;

namespace Draco.Core.Execution.Options
{
    public class OutputObjectOptions : IOutputObjectOptions
    {
        public TimeSpan DefaultTimeoutDuration { get; set; } = TimeSpan.FromHours(1);
    }

    public class OutputObjectOptions<T> : OutputObjectOptions { }
}
