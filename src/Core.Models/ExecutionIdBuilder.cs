// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.Models
{
    public class ExecutionIdBuilder
    {
        public string ExtensionId { get; set; }
        public string ExtensionVersionId { get; set; }
        public string TenantId { get; set; }
        public string ExecutionId { get; set; }

        public static bool TryParse(string source, out ExecutionIdBuilder idBuilder)
        {
            idBuilder = null;

            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            var sourceParts = source.Split('_');

            if (sourceParts.Length != 4)
            {
                return false;
            }

            idBuilder = new ExecutionIdBuilder
            {
                ExtensionId = sourceParts[0],
                ExtensionVersionId = sourceParts[1],
                TenantId = sourceParts[2],
                ExecutionId = sourceParts[3]
            };

            return true;
        }

        public static ExecutionIdBuilder Parse(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceParts = source.Split('_');

            if (sourceParts.Length != 4)
            {
                throw new ArgumentException("Execution Id should consist of four underscore-delimited segments: " +
                                            $"[{nameof(ExtensionId)}_{nameof(ExtensionVersionId)}_{nameof(TenantId)}_{nameof(ExecutionId)}].");
            }

            return new ExecutionIdBuilder
            {
                ExtensionId = sourceParts[0],
                ExtensionVersionId = sourceParts[1],
                TenantId = sourceParts[2],
                ExecutionId = sourceParts[3]
            };
        }

        public override string ToString() =>
            $"{ExtensionId}_{ExtensionVersionId}_{TenantId}_{ExecutionId}";
    }
}
