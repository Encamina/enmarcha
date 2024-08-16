using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Exceptions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text (OCR) and interprets information from images, diagrams and unstructured information. Uses Semantic Kernel.
/// </summary>
public class SkVisionImageDocumentConnector : IEnmarchaDocumentConnector
{
    private const string SystemPrompt = """
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
        [IMAGE]

        2. Represent images or photos using a dedicated Markdown section with a full description of what you see, formatted as follows:
        [PHOTO]

        3. Restrain from adding any additional information or commentary. If the page is empty do not transcribe anything and just return an empty string.

        4. Transcribe only in Markdown format.

        Importance: The fidelity of this transcription is critical. It is essential that the content from the PDF is transcribed exactly as it appears, with no summarization or imprecise descriptions. Accuracy in representing the mixture of text, visual elements, and data is paramount for the success of my project.
        
        """;

    private readonly IChatCompletionService chatCompletionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkVisionImageDocumentConnector"/> class.
    /// </summary>
    /// <param name="kernel">A valid <see cref="Kernel"/> instance.</param>
    public SkVisionImageDocumentConnector(Kernel kernel)
    {
        chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".JPEG", ".JPG", ".PNG"];

    /// <inheritdoc/>
    public virtual string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        var mimeType = ImageHelper.GetMimeType(stream);
        stream.Position = 0;

        var history = new ChatHistory(SystemPrompt);

        var message = new ChatMessageContentItemCollection()
        {
            new ImageContent(BinaryData.FromStream(stream), mimeType),
        };

        history.AddUserMessage(message);

        // TODO: We can improve that making an async version of IEnmarchaDocumentConnector.
        var response = chatCompletionService.GetChatMessageContentAsync(history).GetAwaiter().GetResult();

        // Check if the the model has exceeded the output capacity.
        if (response.Metadata?.TryGetValue(@"FinishReason", out var finishReason) == true &&
            finishReason is string finishReasonString &&
            finishReasonString.Equals(@"length", StringComparison.Ordinal))
        {
            throw new DocumentTooLargeException();
        }

        return response?.Content ?? string.Empty;
    }

    /// <inheritdoc/>
    public virtual void Initialize(Stream stream)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    /// <inheritdoc/>
    public virtual void AppendText(Stream stream, string text)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }
}
