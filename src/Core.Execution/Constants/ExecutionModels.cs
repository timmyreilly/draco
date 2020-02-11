using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.Core.Execution.Constants
{
    public static class ExecutionModels
    {
        public static class Async
        {
            public static class Http
            {
                public static class Json
                {
                    public const string V1 = "http-json/async/v1";
                }
            }
        }

        public static class Sync
        {
            public static class Http
            {
                public static class Json
                {
                    public const string V1 = "http-json/sync/v1";
                }
            }
        }
    }
}
