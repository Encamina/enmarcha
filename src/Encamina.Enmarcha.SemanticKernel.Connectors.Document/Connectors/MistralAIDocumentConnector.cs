using System.Net.Http.Json;
using System.Text;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from a Portable Document File (<c>.pdf</c>) with MistralAI.
/// </summary>
public class MistralAIDocumentConnector : IEnmarchaDocumentConnector
{
    /// <summary>
    /// System prompt used for refine the MistralAI output.
    /// </summary>
    protected const string SystemPrompt = """
        You are an expert assistant specialized in structuring, cleaning, and organizing Markdown documents.

        Your task is to refine and correct content while preserving its original meaning.

        [INSTRUCTIONS]

        1. Maintain and correct Markdown heading hierarchies (#, ##, ###, ####) based on their semantic level.
        2. **COVER PAGE / DOCUMENT TITLE HANDLING**:
           - When you detect a document cover or title page (usually at the beginning), apply the following hierarchy:
             * Main document title: # (H1)
             * Organization/Company name: ## (H2)
             * Subtitle or additional info: ### (H3)
           - Example:
             Input:
                # Title
                Conditions
                ## Organization
                ## Subtitle
             Output:
                # Title
                Conditions
                ## Organization
                ### Subtitle
        3. **TABLE OF CONTENTS vs. REGULAR CONTENT**:
           - In TABLE OF CONTENTS sections: multiple consecutive headings without text between them are normal and expected. Keep them as-is.
           - Outside TABLE OF CONTENTS: if you find multiple consecutive headings at the same level with no content between them, determine if there's a parent-child relationship and adjust hierarchy accordingly.
             Example:
                ## Title of the article
                ## Subtitle of the article
             Should become:
                ## Title of the article
                ### Subtitle of the article
        4. Convert and fix broken tables, lists, and any malformed Markdown formatting.
        5. DO NOT generate hyperlinks, numbered lists where they don't exist, or HTML entities (&nbsp;, <br>, etc.).
        6. Remove headers, footers, page numbers, and any document metadata that appears repeatedly.
        7. Fix OCR errors including split words, incorrect spacing, stray characters, and common OCR mistakes.
        8. DO NOT wrap your output in code fences (```) or add any explanatory text.
        9. DO NOT add, remove, or modify the actual content text. Only fix formatting, structure, and hierarchy.
        10. Return ONLY the final clean, hierarchical, and readable Markdown content without any additional comments or explanations.

        [END INSTRUCTIONS]
        """;

    /// <summary>
    /// The chat completion service instance.
    /// </summary>
    private readonly IChatCompletionService chatCompletionService;

    /// <summary>
    /// Configuration options for MistralAI processing.
    /// </summary>
    private readonly MistralAIDocumentConnectorOptions options;

    /// <summary>
    /// The HTTP client factory for making requests to external services.
    /// </summary>
    private readonly IHttpClientFactory httpFactory;

    /// <summary>
    /// Function to count tokens in a text string.
    /// </summary>
    private readonly Func<string, int> lengthFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAIDocumentConnector"/> class.
    /// </summary>
    /// <param name="kernel">A valid <see cref="Kernel"/> instance.</param>
    /// <param name="options"> A valid instance of <see cref="MistralAIDocumentConnectorOptions"/>.</param>
    /// <param name="httpFactory">The HTTP client factory.</param>
    /// <param name="lengthFunction">Function to count tokens in text.</param>
    public MistralAIDocumentConnector(Kernel kernel, IOptions<MistralAIDocumentConnectorOptions> options, IHttpClientFactory httpFactory, Func<string, int> lengthFunction)
    {
        chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        this.options = options.Value;
        this.httpFactory = httpFactory;
        this.lengthFunction = lengthFunction;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => new[] { ".PDF" };

    /// <inheritdoc/>
    public void AppendText(Stream stream, string text)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    /// <inheritdoc/>
    public void Initialize(Stream stream)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    /// <inheritdoc/>
    public string ReadText(Stream stream) => ReadTextAsync(stream, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Reads text from a PDF stream using the configured AI service.
    /// </summary>
    /// <param name="stream">The PDF stream to read.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The extracted text content.</returns>
    public virtual async Task<string> ReadTextAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(stream);

        var rawMarkdown = await ExtractMarkdownFromPdfAsync(stream, cancellationToken);

        if (options.LLMPostProcessing)
        {
            var refinedMarkdown = await RefineMarkdownWithAIAsync(rawMarkdown, cancellationToken);

            return refinedMarkdown;
        }

        return rawMarkdown;
    }

    /// <summary>
    /// Extracts markdown content from a PDF stream by processing it in chunks.
    /// </summary>
    /// <param name="stream">The PDF stream to process.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The extracted markdown content.</returns>
    private async Task<string> ExtractMarkdownFromPdfAsync(Stream stream, CancellationToken cancellationToken)
    {
        var markdown = new StringBuilder();

        var pdfParts = await MistralAIHelper.SplitPdfByPagesAsync(stream, options.SplitPageNumber, cancellationToken);

        using var httpClient = httpFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromMinutes(10);

        foreach (var pdfPart in pdfParts)
        {
            var markdownPart = await ProcessMarkdownPdfPartAsync(httpClient, pdfPart, cancellationToken);

            if (markdown.Length > 0 && !string.IsNullOrWhiteSpace(markdownPart))
            {
                markdown.AppendLine().AppendLine();
            }

            markdown.Append(markdownPart);
        }

        return markdown.ToString();
    }

    /// <summary>
    /// Processes a single PDF part and extracts markdown.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="pdfPart">The PDF part stream to process.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The extracted markdown content from the PDF part.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    private async Task<string> ProcessMarkdownPdfPartAsync(HttpClient httpClient, Stream pdfPart, CancellationToken cancellationToken)
    {
        var documentUrl = await MistralAIHelper.BuildPdfDataUrlAsync(pdfPart, cancellationToken);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, options.Endpoint)
        {
            Headers = { { "Authorization", $"Bearer {options.ApiKey}" } },
            Content = JsonContent.Create(new
            {
                model = options.ModelName,
                document = new
                {
                    type = "document_url",
                    document_url = documentUrl,
                },
                include_image_base64 = true,
            }),
        };

        using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);

        httpResponse.EnsureSuccessStatusCode();

        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        var combinedMarkdown = MistralAIHelper.ExtractAndCombineMarkdown(content);

        return combinedMarkdown;
    }

    /// <summary>
    /// Refines extracted markdown using AI chat completion by splitting it into manageable parts.
    /// </summary>
    /// <param name="rawMarkdown">The raw markdown to refine.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The refined markdown content.</returns>
    private async Task<string> RefineMarkdownWithAIAsync(string rawMarkdown, CancellationToken cancellationToken)
    {
        var chatModelName = chatCompletionService.Attributes["DeploymentName"];

        var modelInfo = ModelInfo.GetById(chatModelName.ToString());

        var markdownParts = MistralAIHelper.SplitMarkdownForRefinement(rawMarkdown, modelInfo.MaxTokensOutput, lengthFunction);

        var sb = new StringBuilder();

        foreach (var markdownPart in markdownParts)
        {
            var history = new ChatHistory(SystemPrompt);
            history.AddUserMessage(markdownPart);

            var response = await chatCompletionService.GetChatMessageContentAsync(history, cancellationToken: cancellationToken);

            var content = response?.Content ?? string.Empty;

            sb.AppendLine(content);
            sb.AppendLine();
        }

        var refinedMarkdown = sb.ToString().TrimEnd();

        return refinedMarkdown;
    }
}
