// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// TODO: Not sure about this approach to capturing execution model names going forward.
//       It creates a distinction between execution models that come with the platform and those that are added later that 
//       I don't think should be there. Will reevaluate later.

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
