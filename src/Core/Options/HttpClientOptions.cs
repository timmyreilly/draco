using Draco.Core.Interfaces;

namespace Draco.Core.Options
{
    public class HttpClientOptions<T> : IHttpClientOptions
    {
        public int MaximumRetryAttempts { get; set; } = 3;
    }
}
