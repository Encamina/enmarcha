using System.IO.Compression;

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

        // Check if the stream is zlib compressed (common in PDF images)
        var processedStream = TryDecompressZlib(stream) ?? stream;

        try
        {
            using var image = Image.Load(processedStream);

            var mimeType = image.Metadata.DecodedImageFormat?.DefaultMimeType ?? System.Net.Mime.MediaTypeNames.Application.Octet;

            return (mimeType, image.Width, image.Height);
        }
        finally
        {
            // Dispose the decompressed stream if we created one
            if (processedStream != stream)
            {
                processedStream?.Dispose();
            }

            // Reset original stream position if possible
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
        }
    }

    /// <summary>
    /// Creates BinaryData from an image stream, handling potential compression or stream positioning issues.
    /// </summary>
    /// <param name="stream">The image stream.</param>
    /// <returns>BinaryData containing the processed image data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the stream cannot be processed.</exception>
    public static BinaryData CreateImageBinaryData(Stream stream)
    {
        Guard.IsNotNull(stream);

        try
        {
            // Check if the stream is zlib compressed (common in PDF images)
            var processedStream = TryDecompressZlib(stream) ?? stream;

            try
            {
                if (processedStream != stream)
                {
                    // We have a decompressed version, use it
                    using (processedStream)
                    {
                        return BinaryData.FromStream(processedStream);
                    }
                }
                else
                {
                    // Use original stream
                    return BinaryData.FromStream(stream);
                }
            }
            finally
            {
                // Reset original stream position if possible
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
            }
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Failed to create image data from stream.", nameof(stream), ex);
        }
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

    /// <summary>
    /// Attempts to decompress a zlib-compressed stream. Returns the decompressed stream if successful, null otherwise.
    /// </summary>
    /// <param name="stream">The potentially compressed stream.</param>
    /// <returns>A decompressed Stream if the input was zlib-compressed, null otherwise.</returns>
    private static Stream? TryDecompressZlib(Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        // Check if it starts with zlib header
        var buffer = new byte[2];
        var bytesRead = stream.Read(buffer, 0, 2);

        if (bytesRead < 2)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return null;
        }

        // zlib magic numbers: 0x78 followed by 0x9C, 0xDA, or 0x01
        if (buffer[0] != 0x78 || (buffer[1] != 0x9C && buffer[1] != 0xDA && buffer[1] != 0x01))
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return null;
        }

        try
        {
            // Reset to beginning for decompression
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            // Skip the first 2 bytes (zlib header) and decompress with DeflateStream
            stream.ReadByte(); // Skip 0x78
            stream.ReadByte(); // Skip 0x9C (or other)

            using var deflateStream = new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: true);
            var decompressedData = new MemoryStream();
            deflateStream.CopyTo(decompressedData);
            decompressedData.Position = 0;

            return decompressedData;
        }
        catch
        {
            // If decompression fails, reset stream and return null
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return null;
        }
    }
}
