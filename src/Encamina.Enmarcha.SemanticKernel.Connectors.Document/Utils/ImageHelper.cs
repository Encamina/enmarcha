using System.IO.Compression;

using BitMiracle.LibTiff.Classic;

using CommunityToolkit.Diagnostics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

/// <summary>
/// Provides helper methods for working with images.
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// Processes an image stream and returns its MIME type, width, height, and PNG data as a BinaryData object.
    /// </summary>
    /// <param name="stream">The stream containing the image.</param>
    /// <param name="rawCcittWidth">Optional width for raw CCITT images.</param>
    /// <param name="rawCcittHeight">Optional height for raw CCITT images.</param>
    /// <returns>A tuple containing the MIME type, width, height, and PNG data as a BinaryData object.</returns>
    public static (string MimeType, int Width, int Height, BinaryData PngData) ProcessImageAndGetBinary(Stream stream, int? rawCcittWidth = null, int? rawCcittHeight = null)
    {
        Guard.IsNotNull(stream);

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        return ProcessImageStream(stream, (image, mimeType) =>
        {
            var width = image.Width;
            var height = image.Height;

            using var ms = new MemoryStream();
            image.Save(ms, new PngEncoder());
            ms.Position = 0;

            var pngData = BinaryData.FromStream(ms);

            return (mimeType, width, height, pngData);
        }, rawCcittWidth, rawCcittHeight);
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

        // Standard zlib header check
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
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            stream.ReadByte();
            stream.ReadByte();

            using var deflateStream = new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: true);
            var decompressedData = new MemoryStream();
            deflateStream.CopyTo(decompressedData);
            decompressedData.Position = 0;

            return decompressedData;
        }
        catch
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return null;
        }
    }

    /// <summary>
    /// Detects the image type using ImageInfo registrations, or returns "raw-ccitt" if unknown.
    /// </summary>
    /// <param name="stream">The stream containing the image data.</param>
    /// <returns>An <see cref="ImageInfo"/> representing the detected image type.</returns>
    private static ImageInfo DetectImageInfo(Stream stream)
    {
        stream.Position = 0;
        var header = new byte[4];
        var bytesRead = stream.Read(header, 0, 4);
        if (bytesRead != 4)
        {
            throw new InvalidOperationException($"Could not read the expected 4 bytes from the stream (got {bytesRead}).");
        }

        stream.Position = 0;

        var info = ImageInfo.GetAll().FirstOrDefault(x => x.Match(header));

        // If not found, fallback to raw-ccitt
        return info ?? ImageInfo.GetByType("raw-ccitt")!;
    }

    /// <summary>
    /// Ensures the stream is compatible with ImageSharp. Converts PBM/CCITT/TIFF/RAW-CCITT to PNG, leaves others as is.
    /// </summary>
    /// <param name="stream">The image stream to process.</param>
    /// <param name="rawCcittWidth">Optional width for raw CCITT images.</param>
    /// <param name="rawCcittHeight">Optional height for raw CCITT images.</param>
    /// <returns>A stream containing the image in PNG format or the original stream if already compatible.</returns>
    private static Stream EnsureImageSharpCompatible(Stream stream, int? rawCcittWidth = null, int? rawCcittHeight = null)
    {
        var imageInfo = DetectImageInfo(stream);

        switch (imageInfo.Type)
        {
            case "pbm-p4":
                return ConvertPbmP4ToPng(stream);
            case "tiff":
                return ConvertCcittTiffToPng(stream);
            case "raw-ccitt":
                {
                    if (rawCcittWidth is not null && rawCcittHeight is not null)
                    {
                        using var tiffStream = WrapRawCcittAsTiff(stream, rawCcittWidth.Value, rawCcittHeight.Value);
                        return ConvertCcittTiffToPng(tiffStream);
                    }
                    else
                    {
                        throw new ArgumentException("Width and height must be provided for raw-ccitt images.");
                    }
                }

            default:
                stream.Position = 0;

                return stream; // PNG, JPEG, etc.
        }
    }

    /// <summary>
    /// Converts a PBM P4 stream to a PNG stream.
    /// </summary>
    /// <param name="pbmStream"> The PBM P4 stream to convert.</param>
    /// <returns>A stream containing the image in PNG format.</returns>
    private static Stream ConvertPbmP4ToPng(Stream pbmStream)
    {
        pbmStream.Position = 0;

        using var reader = new StreamReader(pbmStream, System.Text.Encoding.ASCII, false, 1024, leaveOpen: true);
        var magic = reader.ReadLine();
        if (magic != "P4")
        {
            throw new ArgumentException("Not a binary PBM P4");
        }

        string? line;
        do
        {
            line = reader.ReadLine();
            if (line == null)
            {
                throw new ArgumentException("Unexpected end of PBM header.");
            }
        }
        while (line.StartsWith('#'));

        var dims = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var width = int.Parse(dims[0]);
        var height = int.Parse(dims[1]);
        var dataOffset = pbmStream.Position;
        pbmStream.Position = dataOffset;
        var bytesPerRow = (width + 7) / 8;
        var bitmap = new byte[bytesPerRow * height];
        var bytesRead = pbmStream.Read(bitmap, 0, bitmap.Length);
        if (bytesRead != bitmap.Length)
        {
            throw new InvalidOperationException($"Could not read all PBM data (expected {bitmap.Length}, got {bytesRead}).");
        }

        using var img = new Image<L8>(width, height);
        for (var y = 0; y < height; y++)
        {
            var rowOffset = y * bytesPerRow;
            for (var x = 0; x < width; x++)
            {
                var bitIndex = x % 8;
                var byteIndex = rowOffset + (x / 8);
                var isBlack = (bitmap[byteIndex] >> (7 - bitIndex) & 0x1) == 0;
                img[x, y] = isBlack ? new L8(0) : new L8(255);
            }
        }

        var pngStream = new MemoryStream();
        img.Save(pngStream, new PngEncoder());
        pngStream.Position = 0;

        return pngStream;
    }

    /// <summary>
    /// Converts a CCITT TIFF stream to a PNG stream.
    /// </summary>
    /// <param name="tiffInput"> The CCITT TIFF stream to convert.</param>
    /// <returns>A stream containing the image in PNG format.</returns>
    private static Stream ConvertCcittTiffToPng(Stream tiffInput)
    {
        tiffInput.Position = 0;

        using var tiff = Tiff.ClientOpen("in-memory", "r", tiffInput, new TiffStream()) ?? throw new ArgumentException("Not a valid TIFF or could not be opened.");
        var width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
        var height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

        var img = new Image<L8>(width, height);
        var buffer = new byte[tiff.ScanlineSize()];
        for (var y = 0; y < height; y++)
        {
            tiff.ReadScanline(buffer, y);
            for (var x = 0; x < width; x++)
            {
                var byteIndex = x / 8;
                var bitIndex = 7 - (x % 8);
                var isBlack = ((buffer[byteIndex] >> bitIndex) & 0x1) == 0;
                img[x, y] = isBlack ? new L8(0) : new L8(255);
            }
        }

        var pngStream = new MemoryStream();
        img.Save(pngStream, new PngEncoder());
        pngStream.Position = 0;

        return pngStream; // The caller must close this stream when done
    }

    /// <summary>
    /// Wraps a raw CCITT stream as a TIFF stream.
    /// </summary>
    /// <param name="rawCcittStream">The raw CCITT stream to wrap.</param>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <returns>A MemoryStream containing the TIFF representation of the raw CCITT data.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the TIFF cannot be created or written to.</exception>
    private static MemoryStream WrapRawCcittAsTiff(Stream rawCcittStream, int width, int height)
    {
        rawCcittStream.Position = 0;
        var rawData = new byte[rawCcittStream.Length];
        var bytesRead = rawCcittStream.Read(rawData, 0, rawData.Length);
        if (bytesRead != rawData.Length)
        {
            throw new InvalidOperationException($"Could not read all of the raw CCITT data (expected {rawData.Length}, got {bytesRead}).");
        }

        var tiffStream = new MemoryStream();
        var tiff = Tiff.ClientOpen("in-memory", "w", tiffStream, new TiffStream()) ?? throw new InvalidOperationException("Could not create in-memory TIFF");
        tiff.SetField(TiffTag.IMAGEWIDTH, width);
        tiff.SetField(TiffTag.IMAGELENGTH, height);
        tiff.SetField(TiffTag.COMPRESSION, Compression.CCITTFAX4);
        tiff.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE);
        tiff.SetField(TiffTag.BITSPERSAMPLE, 1);
        tiff.SetField(TiffTag.SAMPLESPERPIXEL, 1);
        tiff.SetField(TiffTag.ROWSPERSTRIP, height);
        tiff.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

        var strip = 0;
        if (tiff.WriteRawStrip(strip, rawData, rawData.Length) == -1)
        {
            throw new InvalidOperationException("Could not write raw CCITT strip data");
        }

        tiff.WriteDirectory();

        tiffStream.Position = 0;

        return tiffStream;
    }

    /// <summary>
    /// Handles image stream processing, ensuring resource cleanup and compatible format conversion.
    /// </summary>
    /// <typeparam name="T">Return type for the action performed on the image and its MIME type.</typeparam>
    /// <param name="stream">The original image stream.</param>
    /// <param name="action">A function that receives the loaded Image and its MIME type, and returns a value of type T.</param>
    /// <param name="rawCcittWidth">Optional width for raw CCITT images.</param>
    /// <param name="rawCcittHeight">Optional height for raw CCITT images.</param>
    /// <returns>The result of the action.</returns>
    private static T ProcessImageStream<T>(Stream stream, Func<Image, string, T> action, int? rawCcittWidth = null, int? rawCcittHeight = null)
    {
        // Check if the stream is zlib compressed (common in PDF images)
        var processedStream = TryDecompressZlib(stream) ?? stream;

        // Ensure the stream is compatible with ImageSharp
        var compatibleStream = EnsureImageSharpCompatible(processedStream, rawCcittWidth, rawCcittHeight);

        try
        {
            compatibleStream.Position = 0;
            using var image = Image.Load(compatibleStream);
            var info = DetectImageInfo(stream);
            var mimeType = info.MimeType ?? image.Metadata.DecodedImageFormat?.DefaultMimeType ?? System.Net.Mime.MediaTypeNames.Application.Octet;

            return action(image, mimeType);
        }
        finally
        {
            if (processedStream != stream)
            {
                processedStream?.Dispose();
            }

            if (compatibleStream != processedStream)
            {
                compatibleStream?.Dispose();
            }

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
        }
    }
}
