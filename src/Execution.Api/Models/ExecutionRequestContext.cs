using Draco.Core.Models;
using Draco.Execution.Api.Interfaces;
using System.Collections.Generic;

namespace Draco.Execution.Api.Models
{
    public class ExecutionRequestContext<T> : IExecutionRequestContext
    {
        public ExecutionRequestContext() { }

        public ExecutionRequestContext(T originalRequest) =>
            OriginalRequest = originalRequest;

        public T OriginalRequest { get; set; }

        public Core.Models.Execution Execution { get; set; }

        public Extension Extension { get; set; }

        public ExtensionVersion ExtensionVersion { get; set; }

        public ExecutionProfile ExecutionProfile { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
