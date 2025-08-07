using System.Collections.ObjectModel;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Provides information about an image type.
/// </summary>
public sealed class ImageInfo
{
    private static readonly IDictionary<string, ImageInfo> ImageInfoById = new ReadOnlyDictionary<string, ImageInfo>(new Dictionary<string, ImageInfo>()
    {
        { @"pbm-p4", new ImageInfo { Type = @"pbm-p4", MimeType = "image/x-portable-bitmap", Header = [(byte)'P', (byte)'4'], Match = header => header.Length >= 2 && header[0] == (byte)'P' && header[1] == (byte)'4' } },
        { @"tiff-le", new ImageInfo { Type = @"tiff", MimeType = "image/tiff", Header = [0x49, 0x49, 0x2A, 0x00], Match = header => header.Length >= 4 && header[0] == 0x49 && header[1] == 0x49 && header[2] == 0x2A && header[3] == 0x00 } },
        { @"tiff-be", new ImageInfo { Type = @"tiff", MimeType = "image/tiff", Header = [0x4D, 0x4D, 0x00, 0x2A], Match = header => header.Length >= 4 && header[0] == 0x4D && header[1] == 0x4D && header[2] == 0x00 && header[3] == 0x2A } },
        { @"png", new ImageInfo { Type = @"png", MimeType = "image/png", Header = [0x89, 0x50, 0x4E, 0x47], Match = header => header.Length >= 4 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 } },
        { @"jpeg", new ImageInfo { Type = @"jpeg", MimeType = "image/jpeg", Header = [0xFF, 0xD8], Match = header => header.Length >= 2 && header[0] == 0xFF && header[1] == 0xD8 } },
        { @"raw-ccitt", new ImageInfo { Type = @"raw-ccitt", MimeType = "image/tiff", Header = Array.Empty<byte>(), Match = _ => false } },
    });

    /// <summary>
    /// Gets the type of the image.
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Gets the MIME type of the image.
    /// </summary>
    public string MimeType { get; init; }

    /// <summary>
    /// Gets the header data associated with the current object.
    /// </summary>
    public byte[] Header { get; init; }

    /// <summary>
    /// Gets a function that matches the header of an image to determine if it is of the specified type.
    /// </summary>
    public Func<byte[], bool> Match { get; init; }

    /// <summary>
    /// Gets the image information by its type, like for example '<c>png</c>' or '<c>pbm-p4</c>'.
    /// </summary>
    /// <param name="type"> The image's type. For example <c>png</c> or <c>pbm-p4</c>.</param>
    /// <returns>An images information from the given type, or <see langword="null"/> if it is not found.</returns>
    public static ImageInfo? GetByType(string type) => ImageInfoById.TryGetValue(type, out var imageInfo) ? imageInfo : null;

    /// <summary>
    /// Gets all registered image types.
    /// </summary>
    /// <returns>A collection of all registered image information objects.</returns>
    public static IEnumerable<ImageInfo> GetAll() => ImageInfoById.Values;
}
