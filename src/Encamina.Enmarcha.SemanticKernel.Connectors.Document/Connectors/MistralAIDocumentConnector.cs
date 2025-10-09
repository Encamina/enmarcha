using System.Net.Http.Json;
using System.Text;

using CommunityToolkit.Diagnostics;

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

        Your task is to refine and correct the markdown content while preserving its original meaning.

        [INSTRUCTIONS]

        1. Maintain and correct Markdown heading hierarchies (#, ##, ###, ####) based on their semantic level.
        2. **COVER PAGE / DOCUMENT TITLE HANDLING**:
            - When you detect a document cover or title page (usually at the beginning), apply the following hierarchy:
                * Main document title: # (H1)
                * Organization/Company name: ## (H2)
                * Subtitle or additional info: ### (H3)
                Example:
                    Input:
                        # CASER HOGAR INTEGRAL
                        Condiciones Generales y Especiales
                        ## CAJA DE SEGUROS REUNIDOS
                        ## Compañía de Seguros y Reaseguros, S.A. -CASER-
                    Ouput:
                        # CASER HOGAR INTEGRAL
                        Condiciones Generales y Especiales
                        ## CAJA DE SEGUROS REUNIDOS
                        ### Compañía de Seguros y Reaseguros, S.A. -CASER-
        3. **TABLE OF CONTENTS vs. REGULAR CONTENT**:
            - In TABLE OF CONTENTS sections: multiple consecutive headings without text between them are normal and expected. Keep them as-is.
            - Outside TABLE OF CONTENTS: if you find multiple consecutive headings at the same level with no content between them, determine if there's a parent-child relationship and adjust hierarchy accordingly.
              Example:
                Input:
                    ## ARTICLE 21. EXTRAORDINARY RISKS COVERED BY INSURANCE CONSORTIUM
                    ## LEGAL DEFENSE
                Output:
                    ## ARTICLE 21. EXTRAORDINARY RISKS COVERED BY INSURANCE CONSORTIUM
                    ### LEGAL DEFENSE
        4. Convert and fix broken tables, lists, and any malformed Markdown formatting.
        5. DO NOT generate hyperlinks, numbered lists where they don't exist, or HTML entities (&nbsp;, <br>, etc.).
        6. Remove headers, footers, page numbers, and any document metadata that appears repeatedly.
        7. Fix OCR errors including split words, incorrect spacing, stray characters, and common OCR mistakes.
        8. DO NOT wrap your output in code fences (```) or add any explanatory text.
        9. DO NOT add, remove, or modify the actual content text. Only fix formatting, structure, and hierarchy.
        10. Return ONLY the final clean, hierarchical, and readable Markdown content without any additional comments or explanations.
        11. Higher-level headings CANNOT appear under lower-level headings in the hierarchy. For example, an H1 (#) cannot appear under an H2 (##).

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
    /// Initializes a new instance of the <see cref="MistralAIDocumentConnector"/> class.
    /// </summary>
    /// <param name="kernel">A valid <see cref="Kernel"/> instance.</param>
    /// <param name="options"> A valid instance of <see cref="MistralAIDocumentConnectorOptions"/>.</param>
    /// <param name="httpFactory">The HTTP client factory.</param>
    public MistralAIDocumentConnector(Kernel kernel, IOptions<MistralAIDocumentConnectorOptions> options, IHttpClientFactory httpFactory)
    {
        chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        this.options = options.Value;
        this.httpFactory = httpFactory;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".PDF"];

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
    public string ReadText(Stream stream) => ReadTextAsync(stream);

    /// <summary>
    /// Reads text from a PDF stream using the configured AI service.
    /// </summary>
    /// <param name="stream">The PDF stream to read.</param>
    /// <returns>The extracted text content.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public virtual string ReadTextAsync(Stream stream)
    {
        Guard.IsNotNull(stream);

        // Extract raw markdown from PDF
        var rawMarkdown = ExtractMarkdownFromPdf(stream);

        // Refine markdown using chat completion
        var refinedMarkdown = RefineMarkdownWithAI(rawMarkdown);

        return refinedMarkdown;
    }

    /// <summary>
    /// Extracts markdown content from a PDF stream by processing it in chunks.
    /// </summary>
    private string ExtractMarkdownFromPdf(Stream stream)
    {
        var sb = new StringBuilder();

        var pdfParts = MistralAIHelper.SplitPdfByPagesAsync(stream, options.SplitPageNumber, CancellationToken.None).GetAwaiter().GetResult();

        using var httpClient = httpFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromMinutes(10);

        foreach (var pdfPart in pdfParts)
        {
            var markdownPart = ProcessPdfPart(httpClient, pdfPart);

            if (sb.Length > 0 && !string.IsNullOrWhiteSpace(markdownPart))
            {
                sb.AppendLine().AppendLine();
            }

            sb.Append(markdownPart);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Processes a single PDF part and extracts markdown.
    /// </summary>
    private string ProcessPdfPart(HttpClient httpClient, Stream pdfPart)
    {
        var documentUrl = MistralAIHelper.BuildPdfDataUrlAsync(pdfPart, CancellationToken.None).GetAwaiter().GetResult();

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

        using var httpResponse = httpClient.SendAsync(httpRequest, CancellationToken.None).GetAwaiter().GetResult();

        httpResponse.EnsureSuccessStatusCode();

        var content = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        return MistralAIHelper.ExtractAndCombineMarkdown(content);
    }

    /// <summary>
    /// Refines extracted markdown using AI chat completion.
    /// </summary>
    private string RefineMarkdownWithAI(string markdown)
    {
        var splittedMarkdown = MistralAIHelper.SplitMarkdown(markdown);

        var refinedMarkdown = new StringBuilder();

        foreach (var message in splittedMarkdown)
        {
            var history = new ChatHistory(SystemPrompt);
            history.AddUserMessage(message);

            var response = chatCompletionService.GetChatMessageContentAsync(history).GetAwaiter().GetResult();

            var content = response?.Content ?? string.Empty;

            if (refinedMarkdown.Length > 0 && !string.IsNullOrWhiteSpace(content))
            {
                refinedMarkdown.AppendLine().AppendLine();
            }

            refinedMarkdown.Append(content);
        }

        return refinedMarkdown.ToString();
    }
}
