using Draco.Core.ObjectStorage.Interfaces;
using System;

namespace Draco.Core.ObjectStorage.Options
{
    public class ObjectUrlOptions : IObjectUrlOptions
    {
        public TimeSpan DefaultUrlExpirationPeriod { get; set; } = TimeSpan.FromHours(1);
    }

    public class ObjectUrlOptions<T> : ObjectUrlOptions { }
}
