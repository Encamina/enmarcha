using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Exceptions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Extracts text (OCR) and interprets information from images, diagrams and unstructured information. Uses Semantic Kernel.
/// </summary>
public class SkVisionImageExtractor
{
    /// <summary>
    /// System prompt used for OCR and image interpretation.
    /// </summary>
    protected const string SystemPrompt = """
        You are an expert OCR (Optical Character Recognition) Specialist

        You will receive a PDF page containing a mix of text, diagrams, images, tables, and possibly unstructured data.

        Your task is to generate a complete transcription of the PDF page in Markdown format, capturing all content in detail.

        Guidelines:

        - Ensure complete accuracy in transcription; no information should be lost or omitted.
        - Efficiently represent images, diagrams, and unstructured data for later processing by a Language Model.
        - Don't generate any "![]()" links for photos.
        - Extract the data from the graphs as a table in markdown.
        - Approach this task methodically, thinking through each step carefully.
        - This task is crucial for my career success; I rely on your expertise and precision.
        - Ignore any icon image.
        - Never add the following texts:
            - ```markdown
            - ```
        - Specific Instructions for Formatting:

        1. Represent diagrams or schemes using a dedicated section with discrete data in a table in Markdown format, formatted as follows:
        [IMAGE][/IMAGE]

        2. Represent images or photos using a dedicated Markdown section with a full description of what you see, formatted as follows:
        [PHOTO][/PHOTO]

        3. Restrain from adding any additional information or commentary. If the page is empty do not transcribe anything and just return an empty string.

        4. Transcribe only in Markdown format.

        Importance: The fidelity of this transcription is critical. It is essential that the content from the PDF is transcribed exactly as it appears, with no summarization or imprecise descriptions. Accuracy in representing the mixture of text, visual elements, and data is paramount for the success of my project.
        
        """;

    /// <summary>
    /// The chat completion service instance.
    /// </summary>
    private readonly IChatCompletionService chatCompletionService;

    /// <summary>
    /// Configuration options for vision image processing.
    /// </summary>
    private readonly SkVisionImageExtractorOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkVisionImageExtractor"/> class.
    /// </summary>
    /// <param name="kernel">A valid <see cref="Kernel"/> instance.</param>
    /// <param name="options">Configuration options for vision processing.</param>
    public SkVisionImageExtractor(Kernel kernel, IOptions<SkVisionImageExtractorOptions> options)
    {
        chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        this.options = options.Value;
    }

    /// <summary>
    /// Processes an image stream through the vision model to extract text and interpret visual content.
    /// </summary>
    /// <param name="stream">The image stream to process.</param>
    /// <param name="rawCcittWidth"> Optional width for raw CCITT images.</param>
    /// <param name="rawCcittHeight"> Optional height for raw CCITT images.</param>
    /// <returns>The extracted text description from the image.</returns>
    /// <exception cref="DocumentTooLargeException">Thrown when the image exceeds resolution limits or output capacity.</exception>
    public string ProcessImageWithVision(Stream stream, int? rawCcittWidth = null, int? rawCcittHeight = null)
    {
        Guard.IsNotNull(stream);

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        var (mimeType, width, height, pngData) = ImageHelper.ProcessImageAndGetBinary(stream, rawCcittWidth, rawCcittHeight);

        // Check image resolution
        if (width > options.ResolutionLimit || height > options.ResolutionLimit)
        {
            throw new DocumentTooLargeException();
        }

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        var history = new ChatHistory(SystemPrompt);
        var message = new ChatMessageContentItemCollection()
        {
            new ImageContent(pngData, mimeType),
        };

        history.AddUserMessage(message);

        // TODO: We can improve that making an async version of IEnmarchaDocumentConnector
        var response = chatCompletionService.GetChatMessageContentAsync(history).GetAwaiter().GetResult();

        if (response.Metadata?.TryGetValue("FinishReason", out var finishReason) == true &&
            finishReason is string finishReasonString &&
            finishReasonString.Equals("length", StringComparison.OrdinalIgnoreCase))
        {
            throw new DocumentTooLargeException();
        }

        return response?.Content ?? string.Empty;
    }
}
