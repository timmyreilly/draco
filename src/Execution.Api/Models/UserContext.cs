using Draco.Core.Models;
using Draco.Execution.Api.Interfaces;

namespace Draco.Execution.Api.Models
{
    public class UserContext : IUserContext
    {
        public ExecutorContext Executor { get; set; }
    }
}
