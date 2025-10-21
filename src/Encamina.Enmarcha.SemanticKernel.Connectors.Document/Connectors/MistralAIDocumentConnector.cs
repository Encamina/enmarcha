using System.Net.Http.Json;
using System.Text;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.AI.OpenAI.Azure;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from a Portable Document File (<c>.pdf</c>) with MistralAI.
/// </summary>
public class MistralAIDocumentConnector : IEnmarchaDocumentConnector
{
    /// <summary>
    /// System prompt used for refine the MistralAI output.
    /// </summary>
    protected const string SystemPrompt = """"
    You are an expert assistant specialized in structuring, cleaning, and organizing Markdown documents.
    Your task is to refine and correct content while preserving its original meaning.

    ⚠️ CRITICAL RULE #0 - MUST READ FIRST ⚠️

    **PRESERVE ALL CONTENT - NO EXCEPTIONS**
    - You MUST output EVERY SINGLE CHARACTER from the input
    - If the input starts with plain text, lists, or paragraphs BEFORE any heading, you MUST include them at the start of your output
    - If the input starts with a list item (- or *), output it EXACTLY as the first line
    - If the input has text without headers, that is VALID and MUST be preserved
    - NEVER skip content at the beginning just because there's no heading
    - NEVER assume content is "out of context" and omit it
    - You are processing FRAGMENTS of a larger document - incomplete sections are NORMAL and EXPECTED

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

    4. **TABLE CONTINUITY AND MERGING - CRITICAL RULE**:

       **CASE A: Tables with REPEATED IDENTICAL HEADERS**
       - When you find TWO OR MORE CONSECUTIVE tables with IDENTICAL HEADERS (same column names and structure) separated only by blank lines, they represent a SINGLE TABLE split across pages.
       - You MUST merge these tables into ONE continuous table by:
         * Keeping ONLY the FIRST table header and separator
         * REMOVING all subsequent duplicate headers and separators
         * REMOVING all blank lines between table sections
         * Placing all data rows consecutively in a single unified table

       **Example:**
       Input:
       | Col1 | Col2 |
       | :--: | :--: |
       | Item A | Value A |
       | Item B | Value B |

       | Col1 | Col2 |
       | :--: | :--: |
       | Item C | Value C |
       | Item D | Value D |

       Output:
       | Col1 | Col2 |
       | :--: | :--: |
       | Item A | Value A |
       | Item B | Value B |
       | Item C | Value C |
       | Item D | Value D |

       **CASE B: Table rows WITHOUT HEADER (continuation rows)**
       - When you find table rows (lines starting with |) that appear after a complete table WITHOUT a header/separator, these are CONTINUATION ROWS of the previous table.
       - You MUST merge them by:
         * REMOVING all blank lines before the continuation rows
         * Appending the continuation rows directly to the previous table

       **Example:**
       Input:
       | Col1 | Col2 |
       | :--: | :--: |
       | Item A | Value A |
       | Item B | Value B |

       | Item C | Value C |
       | Item D | Value D |

       Output:
       | Col1 | Col2 |
       | :--: | :--: |
       | Item A | Value A |
       | Item B | Value B |
       | Item C | Value C |
       | Item D | Value D |

       - This rule applies ONLY to consecutive tables (tables that follow each other with only blank lines in between).
       - If there is ANY other content (text, headings, etc.) between tables, do NOT merge them.

    5. Convert and fix broken tables, lists, and any malformed Markdown formatting.
    6. DO NOT generate hyperlinks, numbered lists where they don't exist, or HTML entities (&nbsp;, <br>, etc.).
    7. Remove headers, footers, page numbers, and any document metadata that appears repeatedly. Also remove HTML comments that mark page/chunk boundaries (<!-- Chunk X -->).
    8. Fix OCR errors including split words, incorrect spacing, stray characters, and common OCR mistakes.
    9. DO NOT wrap your output in code fences (```) or add any explanatory text.
    10. DO NOT add, remove, or modify the actual content text. Only fix formatting, structure, and hierarchy.
    11. **FINAL REMINDER**: If your input starts with list items, paragraphs, or any text BEFORE a heading, your output MUST also start with that exact content. Check your output's first line matches the input's first line.
    12. Return ONLY the final clean, hierarchical, and readable Markdown content without any additional comments or explanations.

    [END INSTRUCTIONS]
    """";

    /// <summary>
    /// The chat completion service instance.
    /// </summary>
    private readonly IChatCompletionService chatCompletionService;

    /// <summary>
    /// Configuration options for MistralAI Document Connector.
    /// </summary>
    private readonly MistralAIDocumentConnectorOptions mistralAIDocumentConnectorOptions;

    /// <summary>
    /// Configuration options for Azure OpenAI.
    /// </summary>
    private readonly AzureOpenAIOptions azureOpenAIOptions;

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
    /// <param name="mistralAIDocumentConnectorOptions"> A valid instance of <see cref="MistralAIDocumentConnectorOptions"/>.</param>
    /// <param name="azureOpenAIOptions"> A valid instance of <see cref="AzureOpenAIOptions"/>.</param>
    /// <param name="httpFactory">The HTTP client factory.</param>
    /// <param name="lengthFunction">Function to count tokens in text.</param>
    public MistralAIDocumentConnector(Kernel kernel,
                                      IOptions<MistralAIDocumentConnectorOptions> mistralAIDocumentConnectorOptions,
                                      IOptions<AzureOpenAIOptions> azureOpenAIOptions,
                                      IHttpClientFactory httpFactory,
                                      Func<string, int> lengthFunction)
    {
        chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        this.mistralAIDocumentConnectorOptions = mistralAIDocumentConnectorOptions.Value;
        this.azureOpenAIOptions = azureOpenAIOptions.Value;
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

        if (mistralAIDocumentConnectorOptions.LLMPostProcessing)
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

        var pdfParts = await MistralAIHelper.SplitPdfByPagesAsync(stream, mistralAIDocumentConnectorOptions.SplitPageNumber, cancellationToken);

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

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, mistralAIDocumentConnectorOptions.Endpoint)
        {
            Headers = { { "Authorization", $"Bearer {mistralAIDocumentConnectorOptions.ApiKey}" } },
            Content = JsonContent.Create(new
            {
                model = mistralAIDocumentConnectorOptions.ModelName,
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
        if (string.IsNullOrWhiteSpace(azureOpenAIOptions.ChatModelName))
        {
            throw new InvalidOperationException("Chat model name is not configured in the chat completion service.");
        }

        var modelInfo = ModelInfo.GetById(azureOpenAIOptions.ChatModelName) ?? throw new InvalidOperationException($"Model '{azureOpenAIOptions.ChatModelName}' is not registered in ModelInfo.");

        var markdownParts = MistralAIHelper.SplitMarkdownForRefinement(rawMarkdown, modelInfo.MaxTokensOutput, lengthFunction);

        var sb = new StringBuilder();

        foreach (var markdownPart in markdownParts)
        {
            var history = new ChatHistory(SystemPrompt);
            history.AddUserMessage(markdownPart);

            var settings = new OpenAIPromptExecutionSettings()
            {
                Temperature = 0.0f,
            };

            var response = await chatCompletionService.GetChatMessageContentAsync(history, settings, cancellationToken: cancellationToken);

            var content = response?.Content ?? string.Empty;

            sb.AppendLine(content);
            sb.AppendLine();
        }

        var refinedMarkdown = sb.ToString().TrimEnd();

        return refinedMarkdown;
    }
}
