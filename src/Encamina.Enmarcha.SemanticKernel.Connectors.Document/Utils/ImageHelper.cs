using CommunityToolkit.Diagnostics;

using SixLabors.ImageSharp;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

/// <summary>
/// Provides helper methods for working with images.
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// Gets the MIME type of an image from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the image.</param>
    /// <returns>The MIME type of the image, or "application/octet-stream" if the type is unknown.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the stream does not contain a valid image.</exception>
    public static string GetMimeType(Stream stream)
    {
        Guard.IsNotNull(stream);

        var image = Image.Load(stream);

        return image.Metadata.DecodedImageFormat?.DefaultMimeType ?? System.Net.Mime.MediaTypeNames.Application.Octet;
    }
}
