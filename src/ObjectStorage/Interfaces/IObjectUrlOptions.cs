using System;

namespace Draco.Core.ObjectStorage.Interfaces
{
    public interface IObjectUrlOptions
    {
        TimeSpan DefaultUrlExpirationPeriod { get; }
    }
}
