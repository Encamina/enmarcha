namespace Encamina.Enmarcha.Agents;

/// <summary>
/// Static class that defines common constants used in the application.
/// </summary>
public static class CommonConstants
{
    /// <summary>
    /// Correlation items used in the application.
    /// </summary>
    public static class LogAIRequestScopeItems
    {
        /// <summary>
        /// Gets the correlation item key for the activity ID.
        /// </summary>
        public const string ActivityId = @"ActivityId";

        /// <summary>
        /// Gets the correlation item key for the conversation ID.
        /// </summary>
        public const string ConversationId = @"ConversationId";

        /// <summary>
        /// Gets the correlation item key for the user ID.
        /// </summary>
        public const string UserId = @"UserId";

        /// <summary>
        /// Gets the correlation item key for the user email.
        /// </summary>
        public const string UserEmail = @"UserEmail";

        /// <summary>
        /// Gets the endpoint group name for AI-related functionalities.
        /// </summary>
        public const string AI = @"ai";

        /// <summary>
        /// Gets the endpoint group name for management-related functionalities.
        /// </summary>
        public const string Management = @"management";

        /// <summary>
        /// Gets the custom header that represents an activity id.
        /// </summary>
        public const string HeaderActivityId = @"x-avolta-activity-id";

        /// <summary>
        /// Gets the custom header that represents a conversation id.
        /// </summary>
        public const string HeaderConversationId = @"x-avolta-conversation-id";

        /// <summary>
        /// Gets the custom header that represents a user email.
        /// </summary>
        public const string HeaderUserEmail = @"x-avolta-user-email";

        /// <summary>
        /// Gets the custom header that represents a user id.
        /// </summary>
        public const string HeaderUserId = @"x-avolta-user-id";
    }
}
