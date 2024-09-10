using CommunityToolkit.Diagnostics;

using SixLabors.ImageSharp;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

/// <summary>
/// Provides helper methods for working with images.
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// Gets both the MIME type and resolution (width and height) of an image from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the image.</param>
    /// <returns>A tuple containing the MIME type, width, and height of the image.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the stream does not contain a valid image.</exception>
    public static (string MimeType, int Width, int Height) GetImageInfo(Stream stream)
    {
        Guard.IsNotNull(stream);

        using var image = Image.Load(stream);

        var mimeType = image.Metadata.DecodedImageFormat?.DefaultMimeType ?? System.Net.Mime.MediaTypeNames.Application.Octet;

        return (mimeType, image.Width, image.Height);
    }

    /// <summary>
    /// Gets the MIME type of an image from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the image.</param>
    /// <returns>The MIME type of the image, or "application/octet-stream" if the type is unknown.</returns>
    public static string GetMimeType(Stream stream)
    {
        var (mimeType, _, _) = GetImageInfo(stream);
        return mimeType;
    }

    /// <summary>
    /// Gets the resolution (width and height) of an image from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the image.</param>
    /// <returns>A tuple containing the width and height of the image.</returns>
    public static (int Width, int Height) GetResolution(Stream stream)
    {
        var (_, width, height) = GetImageInfo(stream);
        return (width, height);
    }
}
