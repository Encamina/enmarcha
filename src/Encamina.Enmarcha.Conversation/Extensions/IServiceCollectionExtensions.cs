using Encamina.Enmarcha.Conversation;
using Encamina.Enmarcha.Conversation.Abstractions;

using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring common and required services for bots.
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds a Table Storage as repository for localized responses.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="tableConnectionString">The Table Storage connection string.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized responses. Default name <c>Responses</c>.</param>
    /// <param name="intentCounterSeparator">An intent counter separator for scenarios with multiple responses.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the responses provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddTableStorageResponsesProvider(this IServiceCollection services, string defaultLocale, string tableConnectionString, string tableName = @"Responses", string intentCounterSeparator = @"-", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<IIntentResponsesProvider>(serviceLifetime, sp => new TableStorageResponseProvider(tableConnectionString, tableName, defaultLocale, intentCounterSeparator, cacheAbsoluteExpirationSeconds, sp.GetRequiredService<IMemoryCache>()));
    }
}
