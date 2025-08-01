// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace AzureMcp.Core.Services.Telemetry;

internal static class TelemetryConstants
{
    /// <summary>
    /// Name of tags published.
    /// </summary>
    internal class TagName
    {
        public const string AzureMcpVersion = "Version";
        public const string ClientName = "ClientName";
        public const string ClientVersion = "ClientVersion";
        public const string ErrorDetails = "ErrorDetails";
        public const string EventId = "EventId";
        public const string MacAddressHash = "MacAddressHash";
        public const string ResourceHash = "AzResourceHash";
        public const string SubscriptionGuid = "AzSubscriptionGuid";
        public const string ToolName = "ToolName";
    }

    internal class ActivityName
    {
        public const string CommandExecuted = "CommandExecuted";
        public const string ToolExecuted = "ToolExecuted";
    }
}
