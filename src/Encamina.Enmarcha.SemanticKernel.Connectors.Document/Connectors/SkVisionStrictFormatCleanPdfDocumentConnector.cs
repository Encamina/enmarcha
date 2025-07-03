using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using SixLabors.ImageSharp;

using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using UglyToad.PdfPig.Graphics.Colors;
using UglyToad.PdfPig.PdfFonts;

using Page = UglyToad.PdfPig.Content.Page;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// A PDF document connector that combines strict formatting text extraction with vision capabilities
/// to analyze and describe images found in the PDF document.
/// </summary>
public class SkVisionStrictFormatCleanPdfDocumentConnector : StrictFormatCleanPdfDocumentConnector
{
    private readonly SkVisionImageExtractor visionProcessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkVisionStrictFormatCleanPdfDocumentConnector"/> class.
    /// </summary>
    /// <param name="kernel">A valid <see cref="Kernel"/> instance.</param>
    /// <param name="options">Configuration options for vision processing.</param>
    public SkVisionStrictFormatCleanPdfDocumentConnector(
        Kernel kernel,
        IOptions<SkVisionImageDocumentConnectorOptions> options)
    {
        visionProcessor = new SkVisionImageExtractor(kernel, options);
    }

    /// <inheritdoc/>
    protected override IEnumerable<TextBlock> ProcessPageExtensions(Page page)
    {
        var imageTextBlocks = new List<TextBlock>();

        var images = page.GetImages();
        if (!images.Any())
        {
            return imageTextBlocks;
        }

        var imageIndex = 1;
        foreach (var image in images)
        {
            try
            {
                using var imageStream = new MemoryStream(image.RawBytes.ToArray());
                var imageDescription = visionProcessor.ProcessImageWithVision(imageStream);

                if (!string.IsNullOrEmpty(imageDescription))
                {
                    // Create a TextBlock for the image description
                    var imageTextBlock = CreateTextBlockFromText(
                        $"[Image {imageIndex}] {imageDescription}",
                        image.Bounds);

                    imageTextBlocks.Add(imageTextBlock);
                }
            }
            catch (UnknownImageFormatException)
            {
                // Skip image processing if the image format is unknown
            }

            imageIndex++;
        }

        return imageTextBlocks;
    }

    private static TextBlock CreateTextBlockFromText(string text, PdfRectangle bounds)
    {
        var fontDetails = new FontDetails(string.Empty, false, FontDetails.DefaultWeight, false);

        var lines = text.Split('\r', '\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
        {
            lines = [text];
        }

        var textLines = lines.Select(line =>
        {
            var startPoint = new PdfPoint(bounds.BottomLeft.X, bounds.BottomLeft.Y);
            var endPoint = new PdfPoint(bounds.BottomRight.X, bounds.BottomRight.Y);
            var letter = new Letter(
                line, bounds, startPoint, endPoint, bounds.Width,
                12, fontDetails, TextRenderingMode.Fill, GrayColor.Black, GrayColor.Black, 12, 0);

            return new TextLine([new Word([letter])]);
        }).ToList();

        return new TextBlock(textLines);
    }
}
