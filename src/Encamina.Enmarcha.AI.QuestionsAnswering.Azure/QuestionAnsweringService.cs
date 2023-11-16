using Azure;
using Azure.AI.Language.QuestionAnswering;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Azure;

/// <summary>
/// A question answering service powered by the Question Answering feature
/// from Azure Cognitive Service for Language service.
/// </summary>
internal class QuestionAnsweringService : CognitiveServiceBase<QuestionAnsweringServiceOptions>, IQuestionAnsweringService
{
    private readonly QuestionAnsweringClient client;
    private readonly QuestionAnsweringProject project;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionAnsweringService"/> class.
    /// </summary>
    /// <param name="options">Options for this question answering service.</param>
    public QuestionAnsweringService(QuestionAnsweringServiceOptions options) : base(options)
    {
        Options = options;

        client = new QuestionAnsweringClient(options.EndpointUrl, new AzureKeyCredential(options.KeyCredential), new QuestionAnsweringClientOptions()
        {
            DefaultLanguage = Options.KnowledgeBaseLanguage,
        });

        project = new QuestionAnsweringProject(options.KnowledgeBaseName, options.DeploymentSlot);
    }

    /// <inheritdoc/>
    public virtual async Task<QuestionResult> GetAnswersAsync(QuestionRequest request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);
        Guard.IsNotNullOrWhiteSpace(request.Question);

        var options = new AnswersOptions()
        {
            ConfidenceThreshold = Options.ConfidenceThreshold,
            Filters = BuildQueryFilters(request.Options),
            Size = Options.Top,
            UserId = request.UserId,
            RankerKind = GetRankerType(),
        };

        var response = await client.GetAnswersAsync(request.Question, project, options, cancellationToken);

        return new QuestionResult()
        {
            Answers = response.Value.Answers.Select(a => new Answer()
            {
                Value = a.Answer,
                ConfidenceScore = a.Confidence,
                Source = a.Source,
                Id = a.QnaId?.ToString(),
                AssociatedQuestions = a.Questions,
                Metadata = a.Metadata,
            }).ToArray(),
        };
    }

    private static QueryFilters BuildQueryFilters(IQuestionRequestOptions requestOptions)
    {
        if (requestOptions == null)
        {
            return null;
        }

        var queryFilter = new QueryFilters()
        {
            MetadataFilter = BuildMetadataFilter(requestOptions.MetadataOptions),
        };

        if (requestOptions.Sources != null)
        {
            foreach (var source in requestOptions.Sources)
            {
                queryFilter.SourceFilter.Add(source);
            }
        }

        return queryFilter.MetadataFilter != null || queryFilter.SourceFilter.Any() ? queryFilter : null;
    }

    private static MetadataFilter BuildMetadataFilter(MetadataOptions metadataOptions)
    {
        if (metadataOptions == null || (!metadataOptions.Metadata?.Any() ?? true))
        {
            return null;
        }

        var metadataFilter = new MetadataFilter()
        {
            LogicalOperation = ConvertLogicalOperation(metadataOptions.LogicalOperation),
        };

        foreach (var item in metadataOptions.Metadata)
        {
            metadataFilter.Metadata.Add(new MetadataRecord(item.Key, item.Value));
        }

        return metadataFilter;
    }

    private static LogicalOperationKind? ConvertLogicalOperation(LogicalOperation logicalOperation)
    {
        return logicalOperation switch
        {
            LogicalOperation.And => LogicalOperationKind.And,
            LogicalOperation.Or => LogicalOperationKind.Or,
            _ => null,
        };
    }

    private RankerKind? GetRankerType()
    {
        return Options.QuestionAnsweringRankerType switch
        {
            QuestionAnsweringRankerType.QuestionOnly => (RankerKind?)RankerKind.QuestionOnly,
            QuestionAnsweringRankerType.QuestionAndAnswer => (RankerKind?)RankerKind.Default,
            _ => null,
        };
    }
}
