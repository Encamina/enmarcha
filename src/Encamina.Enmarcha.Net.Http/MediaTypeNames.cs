using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Net.Http;

/// <summary>
/// Common media type (formerly known as MIME type) names for file formats and format contents.
/// </summary>
[SuppressMessage("Critical Code Smell",
                 "S2339:Public constant members should not be used",
                 Justification = "Members on this type are expected to be used with Attributes which requires constants (see error CS0182).")]
public static class MediaTypeNames
{
    /// <summary>
    /// Common media type (formerly known as MIME type) names for application specific formats.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Specifies that the Media Type data is an AdaptiveCard Card (<see href="https://adaptivecards.io/"/>).
        /// </summary>
        public const string AdaptiveCard = @"application/vnd.microsoft.card.adaptive";

        /// <summary>
        /// Specifies that the Media Type data is an Excel file (<c>.xlsx</c>).
        /// </summary>
        public const string Excel = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// Specifies that the Media Type data is in JSON format.
        /// </summary>
        public const string Json = @"application/json";

        /// <summary>
        /// Specifies that the Media Type data is not interpreted.
        /// </summary>
        public const string Octet = @"application/octet-stream";

        /// <summary>
        /// Specifies that the Media Type data is in Portable Document Format (PDF).
        /// </summary>
        public const string Pdf = @"application/pdf";

        /// <summary>
        /// Specifies that the Media Type data is in Rich Text Format (RTF).
        /// </summary>
        public const string Rtf = @"application/rtf";

        /// <summary>
        /// Specifies that the Media Type data is a SOAP document.
        /// </summary>
        public const string Soap = @"application/soap+xml";

        /// <summary>
        /// Specifies that the Media Type data is a Tab Separated Values file (<c>.tsv</c>).
        /// </summary>
        public const string Tsv = @"text/tab-separated-values";

        /// <summary>
        /// Specifies that the Media Type data is in XML format.
        /// </summary>
        public const string Xml = @"application/xml";

        /// <summary>
        /// Specifies that the Media Type data is compressed as a zipped file (<c>.zip</c>).
        /// </summary>
        public const string Zip = @"application/zip";
    }

    /// <summary>
    /// Common media type (formerly known as MIME type) names for image data.
    /// </summary>
    public static class Image
    {
        /// <summary>
        /// Specifies that the Media Type data is in Graphics Interchange Format (GIF).
        /// </summary>
        public const string Gif = @"image/gif";

        /// <summary>
        /// Specifies that the Media Type data is in Joint Photographic Experts Group (JPEG) format.
        /// </summary>
        public const string Jpeg = @"image/jpeg";

        /// <summary>
        /// Specifies that the Media Type data is in Tagged Image File Format (TIFF).
        /// </summary>
        public const string Tiff = @"image/tiff";
    }

    /// <summary>
    /// Common media type (formerly known as MIME type) names for text data.
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// Specifies that the Media Type data is in CSV (comma separated values) format.
        /// </summary>
        public const string Csv = @"text/csv";

        /// <summary>
        /// Specifies that the Media Type data is in HTML format.
        /// </summary>
        public const string Html = @"text/html";

        /// <summary>
        /// Specifies that the Media Type data is in plain text format.
        /// </summary>
        public const string Plain = @"text/plain";

        /// <summary>
        /// Specifies that the Media Type data is in Rich Text Format (RTF).
        /// </summary>
        public const string RichText = @"text/richtext";

        /// <summary>
        /// Specifies that the Media Type data is in XML format.
        /// </summary>
        public const string Xml = @"text/xml";
    }
}
