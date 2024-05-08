using System.Globalization;

using Azure;
using Azure.Data.Tables;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Bot.Abstractions.Cards;
using Encamina.Enmarcha.Bot.Abstractions.Greetings;

using Microsoft.Bot.Schema;
using Microsoft.Extensions.Caching.Memory;

namespace Encamina.Enmarcha.Bot.Greetings;

/// <summary>
/// Localized options for greetings messages based on <see cref="HeroCard">hero cards</see>.
/// </summary>
internal class LocalizedHeroCardGreetingsOptionsFromTableStorage : ILocalizedHeroCardGreetingsOptions
{
    private const string CacheKey = @"CacheKey_Greetings";

    private readonly double cacheAbsoluteExpirationSeconds;
    private readonly string tableConnectionString;
    private readonly string tableName;

    private readonly IMemoryCache memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedHeroCardGreetingsOptionsFromTableStorage"/> class.
    /// </summary>
    /// <param name="tableConnectionString">The Table Storage connection string.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a memory cache to improve performance by storing parameters and values retrieved from the Table Storage.</param>
    public LocalizedHeroCardGreetingsOptionsFromTableStorage(string tableConnectionString, string tableName, string defaultLocale, double cacheAbsoluteExpirationSeconds = 86400, IMemoryCache? memoryCache = null)
        : this(tableConnectionString, tableName, CultureInfo.GetCultureInfo(defaultLocale), cacheAbsoluteExpirationSeconds, memoryCache)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedHeroCardGreetingsOptionsFromTableStorage"/> class.
    /// </summary>
    /// <param name="tableConnectionString">The Table Storage connection string.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a memory cache to improve performance by storing parameters and values retrieved from the Table Storage.</param>
    public LocalizedHeroCardGreetingsOptionsFromTableStorage(string tableConnectionString, string tableName, CultureInfo defaultLocale, double cacheAbsoluteExpirationSeconds = 86400, IMemoryCache? memoryCache = null)
    {
        Guard.IsNotNullOrWhiteSpace(tableConnectionString);
        Guard.IsNotNullOrWhiteSpace(tableName);
        Guard.IsNotNullOrWhiteSpace(tableName);
        Guard.IsNotNull(memoryCache);

        this.cacheAbsoluteExpirationSeconds = cacheAbsoluteExpirationSeconds;
        this.tableConnectionString = tableConnectionString;
        this.tableName = tableName;

        this.memoryCache = memoryCache;

        DefaultLocale = defaultLocale;
        LocalizedOptions = BuildOptions();
    }

    /// <inheritdoc/>
    public CultureInfo DefaultLocale { get; }

    /// <inheritdoc/>
    public IDictionary<CultureInfo, IEnumerable<IHeroCardOptions>> LocalizedOptions { get; }

    private IDictionary<CultureInfo, IEnumerable<IHeroCardOptions>> BuildOptions()
    {
        return memoryCache?.GetOrCreate(CacheKey, cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheAbsoluteExpirationSeconds);
            return Init();
        }) ?? Init();
    }

    private IDictionary<CultureInfo, IEnumerable<IHeroCardOptions>> Init()
    {
        var tableClient = new TableClient(tableConnectionString, tableName);
        tableClient.CreateIfNotExists();

        var entities = tableClient.Query<HeroCardGreetingsOptionsTableEntity>().ToLookup(entity => (entity.PartitionKey, entity.Order));

        var dictionary = new Dictionary<string, IList<IHeroCardOptions>>();

        foreach (var entity in entities)
        {
            var option = new InternalHeroCardGreetingsOptions();

            foreach (var item in entity)
            {
                switch (item.Component)
                {
                    case ComponentKind.Title:
                        option.Title = item.Value;
                        break;
                    case ComponentKind.Subtitle:
                        option.Subtitle = item.Value;
                        break;
                    case ComponentKind.Text:
                        option.Text = item.Value;
                        break;
                    case ComponentKind.ImageUrl:
                        option.Images.Insert(item.ImageOrder, new CardImage(item.Value));
                        break;
                }
            }

            if (dictionary.ContainsKey(entity.Key.PartitionKey))
            {
                dictionary[entity.Key.PartitionKey].Add(option);
            }
            else
            {
                dictionary[entity.Key.PartitionKey] = new List<IHeroCardOptions>() { option };
            }
        }

        return dictionary.ToDictionary(k => CultureInfo.GetCultureInfo(k.Key), e => e.Value.AsEnumerable());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "This is a private enum type that should not be exposed externally, and rarely modified.")]
    private enum ComponentKind
    {
        Unknown,
        Title,
        Subtitle,
        Text,
        ImageUrl,
    }

    private sealed class InternalHeroCardGreetingsOptions : IHeroCardOptions
    {
        private readonly List<CardImage> cardImages = new();

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Text { get; set; }

        public IList<CardImage> Images => cardImages;
    }

    private sealed class HeroCardGreetingsOptionsTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public ComponentKind Component { get; set; } = ComponentKind.Unknown;

        public string Value { get; set; } = string.Empty;

        public int Order { get; set; } = 0;

        public int ImageOrder { get; set; } = 0;
    }
}
