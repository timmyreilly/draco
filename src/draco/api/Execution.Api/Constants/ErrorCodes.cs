// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Execution.Api.Constants
{
    public static class ErrorCodes
    {
        public const string ExtensionIdNotProvided = "E100";
        public const string ExtensionVersionIdNotProvided = "E101";
        public const string InvalidPriority = "E102";
        public const string ExtensionNotFound = "E103";
        public const string ExtensionDisabled = "E104";
        public const string ExtensionVersionNotFound = "E105";
        public const string ExtensionVersionDisabled = "E106";
        public const string ExtensionVersionDoesNotSupportValidation = "E107";
        public const string ExecutionProfileNotFound = "E108";
        public const string PriorityNotSupported = "E109";
        public const string ExecutionNotFound = "E110";
        public const string InvalidExecutionStatus = "E111";
        public const string InvalidExecutionStatusTransition = "E112";
        public const string UnknownInputObjects = "E113";
        public const string MissingInputObjects = "E114";
        public const string UnableToContinue = "E115";
    }
}
