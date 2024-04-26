using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using Azure;
using Azure.Data.Tables;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;
using Encamina.Enmarcha.Core.Extensions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Azure.Metadata;

/// <summary>
/// Provides metadata handling using parameters stored in an Azure Table Storage with the optional
/// posibility to cached these parameters to imporve performance.
/// </summary>
internal class CachedTableStorageCompositeMetadataHandler : IMetadataHandler
{
    private const string CacheKey = @"CacheKey_Metadata";
    private const string RegexFormat = @"(?:^|\W){0}(?:$|\W)|";

    private readonly CachedTableStorageCompositeMetadataHandlerOptions options;
    private readonly IMemoryCache? memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedTableStorageCompositeMetadataHandler"/> class.
    /// </summary>
    /// <param name="options">Configuration options for this specific metadata handler.</param>
    /// <param name="memoryCache">An optional valid instance of a memory cache to improve performance by storing parameters and values retrieved from the Table Storage.</param>
    public CachedTableStorageCompositeMetadataHandler(IOptions<CachedTableStorageCompositeMetadataHandlerOptions> options, IMemoryCache? memoryCache = null)
    {
        this.memoryCache = memoryCache;
        this.options = options?.Value;

        Guard.IsNotNull(this.options);
        Guard.IsNotNullOrWhiteSpace(this.options.TableConnectionString);
    }

    /// <inheritdoc/>
    public int Order => options.Order;

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<IAnswer>> HandleAnswersAsync(IEnumerable<IAnswer> answers, MetadataOptions metadataOptions, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyCollection<IAnswer>>(answers.ToArray());

    /// <inheritdoc/>
    public async Task<MetadataOptions> HandleMessageAsync(string message, MetadataOptions currentMetadataOptions, CancellationToken cancellationToken)
    {
        currentMetadataOptions ??= new MetadataOptions();

        currentMetadataOptions.Metadata.AddRange((memoryCache == null
            ? await InitAsync(cancellationToken)
            : await memoryCache.GetOrCreateAsync(CacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(options.CacheAbsoluteExpirationSeconds);
                return InitAsync(cancellationToken);
            })).Where(i => i.Value.IsMatch(message)).Select(i => i.Key));

        currentMetadataOptions.LogicalOperation = options.MetadataLogicalOperation;

        return currentMetadataOptions;
    }

    private static Regex BuildRegex(string terms, string termsSeparatorToken)
    {
        var patternStringBuilder = new StringBuilder();

        var splitedTerms = terms.Split(termsSeparatorToken, StringSplitOptions.RemoveEmptyEntries);

        foreach (var splitedTerm in splitedTerms)
        {
            patternStringBuilder.AppendFormat(CultureInfo.InvariantCulture, RegexFormat, splitedTerm);
        }

        patternStringBuilder.Length--; // Simplest and most efficient way to remove the trailing '|' from the ´RegexFormat´ constant...

        return new Regex(patternStringBuilder.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    private async Task<IDictionary<KeyValuePair<string, string>, Regex>> InitAsync(CancellationToken cancellationToken)
    {
        var tableClient = new TableClient(options.TableConnectionString, options.TableName);
        await tableClient.CreateIfNotExistsAsync(cancellationToken);

        return await tableClient.QueryAsync<MetadataTableEntity>(cancellationToken: cancellationToken)
                                .ToDictionaryAsync(e => KeyValuePair.Create(e.IsComposite ? $@"{e.Label}{e.CompositeToken}{e.Value}" : e.Label, e.Value),
                                                   e => BuildRegex(e.Terms, e.TermsSeparatorToken),
                                                   cancellationToken);
    }

    private sealed class MetadataTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public bool IsComposite { get; set; } = false;

        public string CompositeToken { get; set; } = string.Empty;

        public string Label => PartitionKey;

        public string Value => RowKey;

        public string Terms { get; set; } = string.Empty;

        public string TermsSeparatorToken { get; set; } = string.Empty;
    }
}
