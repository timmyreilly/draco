using System;
using System.Collections.Generic;

namespace Draco.Core.Interfaces
{
    public interface INamedServiceFactory<TService> : IDictionary<string, Func<IServiceProvider, TService>> { }
}
