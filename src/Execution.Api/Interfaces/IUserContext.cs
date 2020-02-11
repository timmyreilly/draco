using Draco.Core.Models;

namespace Draco.Execution.Api.Interfaces
{
    public interface IUserContext
    {
        ExecutorContext Executor { get; }
    }
}
