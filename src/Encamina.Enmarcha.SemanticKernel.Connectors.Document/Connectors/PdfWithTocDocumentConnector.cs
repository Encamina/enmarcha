// Ignore Spelling: pdf
// Ignore Spelling: toc

using System.Text;
using System.Text.RegularExpressions;

using CommunityToolkit.Diagnostics;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Outline;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from a Portable Document File (<c>.pdf</c>) that includes a Table of Contents. Cleaning common words that overlap on pages.
/// For each item in the table of contents, extract the associated text.
/// </summary>
/// <remarks>
/// During the text extraction process, it's important to note that the generated text for each section does not preserve the original structure
/// of the PDF document, such as paragraphs and other formatting elements. Instead, the text for each section is generated as a single paragraph.
/// </remarks>
public class PdfWithTocDocumentConnector : CleanPdfDocumentConnector
{
    private static readonly Regex RemoveSpacesRegex = new(@"\s+", RegexOptions.Compiled, TimeSpan.FromSeconds(30));

    /// <summary>
    /// Gets the function used to format a Table of Contents (TOC) item into a string representation.
    /// </summary>
    public Func<TocItem, string> TocItemFormat { get; init; } = (tocItem) => $"{tocItem.Title}: {tocItem.Content}";

    /// <inheritdoc/>
    public override string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        using var document = PdfDocument.Open(stream);

        if (!document.TryGetBookmarks(out var bookMarks))
        {
            return string.Empty;
        }

        // 1. Extract Table of Contents
        var tocItems = bookMarks.GetNodes()
            .OfType<DocumentBookmarkNode>()
            .Select(node => new TocItem { PageNumber = node.PageNumber, Title = node.Title })
            .ToList();

        // 2. Create and extract content (text) from each pages
        var pages = CreatePages(document);

        // 3. Add content (text) to each toc item
        AddTocContent(tocItems, pages);

        return string.Join(Environment.NewLine, tocItems.Select(TocItemFormat));
    }

    private static List<Page> CreatePages(PdfDocument document)
    {
        var pages = document.GetPages()
            .Select(page => new Page { Number = page.Number, Words = page.GetWords() })
            .ToList();

        var documentWords = pages
            .SelectMany(p => p.Words)
            .Select(word => new WordElement(word))
            .ToList();

        var commonWords = GetCommonTextElements(document, documentWords);

        foreach (var page in pages)
        {
            var pageWords = page.Words.Select(word => new WordElement(word)).ToList();
            var nonCommonWordsText = RemoveCommonTextElements(pageWords, commonWords);

            page.Content = RemoveExtraSpaces(string.Join(' ', nonCommonWordsText)).Trim();
        }

        return pages;
    }

    private static void AddTocContent(List<TocItem> tocItems, IReadOnlyCollection<Page> pages)
    {
        foreach (var tocItem in tocItems)
        {
            var currentTitle = RemoveExtraSpaces(tocItem.Title);
            var currentPage = pages.First(p => p.Number == tocItem.PageNumber);
            var currentIndex = currentPage.Content.LastIndexOf(currentTitle, StringComparison.Ordinal) + currentTitle.Length;

            string tocContent;
            var nextTocItemIndex = tocItems.IndexOf(tocItem) + 1;

            if (nextTocItemIndex < tocItems.Count)
            {
                // It is not the last title

                var nextTocItem = tocItems[nextTocItemIndex];
                var nextTitle = RemoveExtraSpaces(nextTocItem.Title);
                var nextPage = pages.First(p => p.Number == nextTocItem.PageNumber);
                var nextIndex = nextPage.Content.LastIndexOf(nextTitle, StringComparison.Ordinal);

                tocContent = GetTocContent(pages, currentPage, currentIndex, nextPage, nextIndex);
            }
            else
            {
                // It is the last title. Extract the text to the end.

                var lastPage = pages.MaxBy(p => p.Number);
                var lastIndex = lastPage.Content.Length;

                tocContent = GetTocContent(pages, currentPage, currentIndex, lastPage, lastIndex);
            }

            tocItem.Content = tocContent.Trim();
        }
    }

    private static string GetTocContent(IReadOnlyCollection<Page> pages, Page currentPage, int currentIndex, Page nextPage, int nextIndex)
    {
        // Current and next item are on the same page
        if (currentPage.Number == nextPage.Number)
        {
            // Extract the text on the current page
            return currentPage.Content[currentIndex..nextIndex];
        }

        // Current and next item are on different page
        // Extract the text from the current page, the pages in between,
        // and the page where the next item is located
        var textContent = new StringBuilder();

        textContent.Append($"{currentPage.Content[currentIndex..]} ");

        for (var i = currentPage.Number + 1; i < nextPage.Number; i++)
        {
            textContent.Append($"{pages.First(p => p.Number == i).Content} ");
        }

        textContent.Append(nextPage.Content[..nextIndex]);

        return textContent.ToString();
    }

    private static string RemoveExtraSpaces(string text)
    {
        return RemoveSpacesRegex.Replace(text, " ");
    }

    /// <summary>
    /// Represents an item in a Table of Contents.
    /// </summary>
    public class TocItem
    {
        /// <summary>
        /// Gets the page number associated with this TOC item.
        /// </summary>
        public int PageNumber { get; init; }

        /// <summary>
        /// Gets the title of this TOC item.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Gets or sets the content associated with this TOC item.
        /// </summary>
        public string Content { get; set; }
    }

    private sealed class Page
    {
        public int Number { get; init; }

        public IEnumerable<Word> Words { get; init; }

        public string Content { get; set; }
    }
}
