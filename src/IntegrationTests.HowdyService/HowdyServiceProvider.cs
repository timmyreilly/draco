using Draco.Core.Models;
using Draco.Core.Services.Interfaces;
using Draco.Core.Services.Providers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Draco.IntegrationTests.HowdyService
{
    public class HowdyServiceProvider : BaseExecutionServiceProvider, IExecutionServiceProvider
    {
        public override Task<JObject> GetServiceConfigurationAsync(ExecutionRequest execRequest) =>
            Task.FromResult(JObject.FromObject(new HowdyServiceConfiguration(
                $"Howdy, user {execRequest.Executor.UserId}! This service doesn't do much. It does, however, " +
                 "demonstrate how execution services work within the context of an execution request.")));

        public class HowdyServiceConfiguration
        {
            public HowdyServiceConfiguration() { }

            public HowdyServiceConfiguration(string message)
            {
                Message = message;
            }

            public string Message { get; set; }
        }
    }
}
