// Ignore Spelling: pdf

using CommunityToolkit.Diagnostics;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.DocumentLayoutAnalysis;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from a Portable Document File (<c>.pdf</c>), cleaning common words that overlap on pages.
/// </summary>
/// <remarks>
/// During the text extraction process, it's important to note that the generated text for each section does not preserve the original structure
/// of the PDF document, such as paragraphs and other formatting elements. Instead, the text for each section is generated as a single paragraph.
/// For strict format text extraction, consider using <see cref="StrictFormatCleanPdfDocumentConnector"/>.
/// </remarks>
public class CleanPdfDocumentConnector : PdfDocumentConnector
{
    /// <inheritdoc/>
    public override string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        using var document = PdfDocument.Open(stream);

        var documentWords = document.GetPages()
            .SelectMany(p => p.GetWords())
            .Select(word => new WordElement(word));

        var nonCommonWordsText = RemoveCommonTextElements(document, documentWords);

        return string.Join(' ', nonCommonWordsText).Trim();
    }

    /// <summary>
    /// Removes common text elements from a collection of text elements based on their repetition in the same position throughout the document.
    /// </summary>
    /// <typeparam name="T">The type of text elements (e.g., Word, TextBlock).</typeparam>
    /// <param name="document">The PDF document.</param>
    /// <param name="elements">The collection of text elements from which to remove common elements.</param>
    /// <returns>An IEnumerable containing the non-common text elements.</returns>
    protected static IEnumerable<T> RemoveCommonTextElements<T>(PdfDocument document, IEnumerable<TextElement<T>> elements)
    {
        var textElements = elements.ToList();

        var commonTextElements = GetCommonTextElements(document, textElements);

        return RemoveCommonTextElements(textElements, commonTextElements);
    }

    /// <summary>
    /// Removes common text elements from a collection of text elements based on a list of common elements.
    /// </summary>
    /// <typeparam name="T">The type of text elements (e.g., Word, TextBlock).</typeparam>
    /// <param name="elements">The collection of text elements from which to remove common elements.</param>
    /// <param name="commonElements">The list of common text elements to be removed.</param>
    /// <returns>An IEnumerable containing the non-common text elements.</returns>
    protected static IEnumerable<T> RemoveCommonTextElements<T>(IEnumerable<TextElement<T>> elements, IEnumerable<TextElement<T>> commonElements)
    {
        var textElements = elements.ToList();

        var nonCommonTextElements = textElements.Where(textElement => !IsCommonTextElement(textElement, commonElements)).Select(textElement => textElement.Element);

        return nonCommonTextElements;
    }

    /// <summary>
    /// Retrieves common text elements from a PDF document. It is based on the repetition of the same text element in the same position throughout the document.
    /// </summary>
    /// <typeparam name="T">The type of text elements (e.g., Word, TextBlock).</typeparam>
    /// <param name="document">The PDF document.</param>
    /// <param name="elements">The list of text elements from which to retrieve common elements.</param>
    /// <returns>A collection of common text elements.</returns>
    protected static IList<TextElement<T>> GetCommonTextElements<T>(PdfDocument document, IEnumerable<TextElement<T>> elements)
    {
        // We want to remove common words that overlap on pages, so we need to find the minimum number of overlaps a word must have to be considered common.
        // In this case, we will consider a word to be common if it overlaps on at least a quarter of its pages.
        var minOverlaps = document.NumberOfPages / 4;

        // Edge case...
        if (minOverlaps < 2)
        {
            minOverlaps = 2;
        }

        var result = new List<TextElement<T>>();
        var textElementCount = new Dictionary<TextElement<T>, int>();
        var textElements = elements.ToList();

        for (var i = 0; i < textElements.Count; i++)
        {
            var currentTextElement = textElements[i];

            for (var j = i + 1; j < textElements.Count; j++)
            {
                var nextTextElement = textElements[j];

                if (currentTextElement.Text == nextTextElement.Text && RectanglesOverlap(currentTextElement.BoundingBox, nextTextElement.BoundingBox))
                {
                    IncrementTextElementCount(textElementCount, currentTextElement);
                }
            }
        }

        foreach (var textElement in textElementCount.Keys)
        {
            if (textElementCount[textElement] >= minOverlaps)
            {
                result.Add(textElement);
            }
        }

        return result;
    }

    /// <summary>
    /// Checks if a given word is a common word by comparing it with a collection of common words.
    /// </summary>
    /// <typeparam name="T">The type of text elements (e.g., Word, TextBlock).</typeparam>
    /// <param name="textElement">The word to check.</param>
    /// <param name="commonTextElements">The collection of common words to compare against.</param>
    /// <returns>True if the word is common; otherwise, false.</returns>
    protected static bool IsCommonTextElement<T>(TextElement<T> textElement, IEnumerable<TextElement<T>> commonTextElements)
    {
        return commonTextElements.Any(w => w.Text == textElement.Text && w.BoundingBox.Equals(textElement.BoundingBox));
    }

    private static void IncrementTextElementCount<T>(IDictionary<TextElement<T>, int> textElementCount, TextElement<T> currentTextElement)
    {
        if (textElementCount.ContainsKey(currentTextElement))
        {
            textElementCount[currentTextElement]++;
        }
        else
        {
            textElementCount[currentTextElement] = 1;
        }
    }

    private static bool RectanglesOverlap(PdfRectangle rect1, PdfRectangle rect2)
    {
        return rect1.Left < rect2.Right && rect1.Right > rect2.Left && rect1.Bottom < rect2.Top && rect1.Top > rect2.Bottom;
    }

    /// <summary>
    /// Represents a base class for text elements, such as TextBlock or Word.
    /// </summary>
    /// <typeparam name="T">The specific type of text element.</typeparam>
    protected class TextElement<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextElement{T}"/> class with the specified element.
        /// </summary>
        /// <param name="element">The specific text element.</param>
        protected TextElement(T element)
        {
            Element = element;
        }

        /// <summary>
        /// Gets the text content of the text element.
        /// </summary>
        public string Text { get; protected init; }

        /// <summary>
        /// Gets the bounding box of the text element.
        /// </summary>
        public PdfRectangle BoundingBox { get; protected init; }

        /// <summary>
        /// Gets the specific text element.
        /// </summary>
        public T Element { get; }
    }

    /// <summary>
    /// Represents a concrete class for a Word text element.
    /// </summary>
    protected sealed class WordElement : TextElement<Word>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordElement"/> class with the specified Word element.
        /// </summary>
        /// <param name="word">The specific Word element.</param>
        public WordElement(Word word) : base(word)
        {
            Text = word.Text;
            BoundingBox = word.BoundingBox;
        }
    }

    /// <summary>
    /// Represents a concrete class for a TextBlock text element.
    /// </summary>
    protected sealed class TextBlockElement : TextElement<TextBlock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlockElement"/> class with the specified TextBlock element.
        /// </summary>
        /// <param name="textBlock">The specific Word element.</param>
        public TextBlockElement(TextBlock textBlock) : base(textBlock)
        {
            Text = textBlock.Text;
            BoundingBox = textBlock.BoundingBox;
        }
    }
}
