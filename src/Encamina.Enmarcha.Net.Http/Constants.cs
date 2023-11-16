namespace Encamina.Enmarcha.Net.Http;

/// <summary>
/// A collection of properties that represent common constant values.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Custom HTTP Headers.
    /// </summary>
    public static class HttpHeaders
    {
        /// <summary>
        /// Gets the custom header that represents a correlation id.
        /// </summary>
        /// <remarks>
        /// A unique ID (like a <see cref="Guid"/>, <see cref="string"/> or <see cref="int"/>) that identifies a call chain (for example in logs).
        /// It might help correlating calls and logs between systems and servers. For example, if you make several outgoing calls to service one call
        /// from your consumers, this value will be the same for all those calls.
        /// </remarks>
        public static string CorrelationId => @"x-correlation-id";

        /// <summary>
        /// Gets the custom header that represents a correlation call id.
        /// </summary>
        /// <remarks>
        /// A unique ID (like a <see cref="Guid"/>, <see cref="string"/> or <see cref="int"/>) that identifies a specific call among other calls (for example in logs) in a chain call.
        /// It might help correlating logs between systems and servers. For example, your node or service might make several outgoing calls for to serve a single call from a consumer,
        /// so in this scenario, this value will be the unique ID that separates a specific single call from the other additional calls.
        /// </remarks>
        public static string CorrelationCallId => @"x-correlation-call-id";

        /// <summary>
        /// Gets the custom header that represents a source URL.
        /// </summary>
        public static string SourceUrl => @"x-source-url";
    }
}
