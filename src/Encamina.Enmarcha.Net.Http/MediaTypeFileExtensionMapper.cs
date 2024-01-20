using System.Collections.ObjectModel;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Core.Extensions;

namespace Encamina.Enmarcha.Net.Http;

/// <summary>
/// Provides a mapping between file extensions and media types (former MIME).
/// </summary>
/// <remarks>
/// The main difference with <see href="https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/StaticFiles/src/FileExtensionContentTypeProvider.cs">FileExtensionContentTypeProvider</see>
/// is that this considers various media types per extension, which is useful for scenarios like for example zip files which can be identified as <c>"application/zip",</c>
/// <c>"application/zip-compressed"</c>, or <c>"application/x-zip-compressed"</c>.
/// </remarks>
public sealed class MediaTypeFileExtensionMapper
{
    /// <summary>
    /// Default mapping between extensions and media types.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, IEnumerable<string>> DefaultMap = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
    {
        { @".323", new[] { @"text/h323" } },
        { @".3g2", new[] { @"video/3gpp2" } },
        { @".3gp2", new[] { @"video/3gpp2" } },
        { @".3gp", new[] { @"video/3gpp" } },
        { @".3gpp", new[] { @"video/3gpp" } },
        { @".aac", new[] { @"audio/aac" } },
        { @".aaf", new[] { @"application/octet-stream" } },
        { @".aca", new[] { @"application/octet-stream" } },
        { @".accdb", new[] { @"application/msaccess" } },
        { @".accde", new[] { @"application/msaccess" } },
        { @".accdt", new[] { @"application/msaccess" } },
        { @".acx", new[] { @"application/internet-property-stream" } },
        { @".adt", new[] { @"audio/vnd.dlna.adts" } },
        { @".adts", new[] { @"audio/vnd.dlna.adts" } },
        { @".afm", new[] { @"application/octet-stream" } },
        { @".ai", new[] { @"application/postscript" } },
        { @".aif", new[] { @"audio/x-aiff" } },
        { @".aifc", new[] { @"audio/aiff" } },
        { @".aiff", new[] { @"audio/aiff" } },
        { @".appcache", new[] { @"text/cache-manifest" } },
        { @".application", new[] { @"application/x-ms-application" } },
        { @".art", new[] { @"image/x-jg" } },
        { @".asd", new[] { @"application/octet-stream" } },
        { @".asf", new[] { @"video/x-ms-asf" } },
        { @".asi", new[] { @"application/octet-stream" } },
        { @".asm", new[] { @"text/plain" } },
        { @".asr", new[] { @"video/x-ms-asf" } },
        { @".asx", new[] { @"video/x-ms-asf" } },
        { @".atom", new[] { @"application/atom+xml" } },
        { @".au", new[] { @"audio/basic" } },
        { @".avi", new[] { @"video/x-msvideo" } },
        { @".axs", new[] { @"application/olescript" } },
        { @".bas", new[] { @"text/plain" } },
        { @".bcpio", new[] { @"application/x-bcpio" } },
        { @".bin", new[] { @"application/octet-stream" } },
        { @".bmp", new[] { @"image/bmp" } },
        { @".c", new[] { @"text/plain" } },
        { @".cab", new[] { @"application/vnd.ms-cab-compressed" } },
        { @".calx", new[] { @"application/vnd.ms-office.calx" } },
        { @".cat", new[] { @"application/vnd.ms-pki.seccat" } },
        { @".cdf", new[] { @"application/x-cdf" } },
        { @".chm", new[] { @"application/octet-stream" } },
        { @".class", new[] { @"application/x-java-applet" } },
        { @".clp", new[] { @"application/x-msclip" } },
        { @".cmx", new[] { @"image/x-cmx" } },
        { @".cnf", new[] { @"text/plain" } },
        { @".cod", new[] { @"image/cis-cod" } },
        { @".cpio", new[] { @"application/x-cpio" } },
        { @".cpp", new[] { @"text/plain" } },
        { @".crd", new[] { @"application/x-mscardfile" } },
        { @".crl", new[] { @"application/pkix-crl" } },
        { @".crt", new[] { @"application/x-x509-ca-cert" } },
        { @".csh", new[] { @"application/x-csh" } },
        { @".css", new[] { @"text/css" } },
        { @".csv", new[] { @"text/csv" } },
        { @".cur", new[] { @"application/octet-stream" } },
        { @".dcr", new[] { @"application/x-director" } },
        { @".deploy", new[] { @"application/octet-stream" } },
        { @".der", new[] { @"application/x-x509-ca-cert" } },
        { @".dib", new[] { @"image/bmp" } },
        { @".dir", new[] { @"application/x-director" } },
        { @".disco", new[] { @"text/xml" } },
        { @".dlm", new[] { @"text/dlm" } },
        { @".doc", new[] { @"application/msword" } },
        { @".docm", new[] { @"application/vnd.ms-word.document.macroEnabled.12" } },
        { @".docx", new[] { @"application/vnd.openxmlformats-officedocument.wordprocessingml.document" } },
        { @".dot", new[] { @"application/msword" } },
        { @".dotm", new[] { @"application/vnd.ms-word.template.macroEnabled.12" } },
        { @".dotx", new[] { @"application/vnd.openxmlformats-officedocument.wordprocessingml.template" } },
        { @".dsp", new[] { @"application/octet-stream" } },
        { @".dtd", new[] { @"text/xml" } },
        { @".dvi", new[] { @"application/x-dvi" } },
        { @".dvr-ms", new[] { @"video/x-ms-dvr" } },
        { @".dwf", new[] { @"drawing/x-dwf" } },
        { @".dwp", new[] { @"application/octet-stream" } },
        { @".dxr", new[] { @"application/x-director" } },
        { @".eml", new[] { @"message/rfc822" } },
        { @".emz", new[] { @"application/octet-stream" } },
        { @".eot", new[] { @"application/vnd.ms-fontobject" } },
        { @".eps", new[] { @"application/postscript" } },
        { @".etx", new[] { @"text/x-setext" } },
        { @".evy", new[] { @"application/envoy" } },
        { @".exe", new[] { @"application/vnd.microsoft.portable-executable" } },
        { @".fdf", new[] { @"application/vnd.fdf" } },
        { @".fif", new[] { @"application/fractals" } },
        { @".fla", new[] { @"application/octet-stream" } },
        { @".flr", new[] { @"x-world/x-vrml" } },
        { @".flv", new[] { @"video/x-flv" } },
        { @".gif", new[] { @"image/gif" } },
        { @".gtar", new[] { @"application/x-gtar" } },
        { @".gz", new[] { @"application/x-gzip" } },
        { @".h", new[] { @"text/plain" } },
        { @".hdf", new[] { @"application/x-hdf" } },
        { @".hdml", new[] { @"text/x-hdml" } },
        { @".hhc", new[] { @"application/x-oleobject" } },
        { @".hhk", new[] { @"application/octet-stream" } },
        { @".hhp", new[] { @"application/octet-stream" } },
        { @".hlp", new[] { @"application/winhlp" } },
        { @".hqx", new[] { @"application/mac-binhex40" } },
        { @".hta", new[] { @"application/hta" } },
        { @".htc", new[] { @"text/x-component" } },
        { @".htm", new[] { @"text/html" } },
        { @".html", new[] { @"text/html" } },
        { @".htt", new[] { @"text/webviewhtml" } },
        { @".hxt", new[] { @"text/html" } },
        { @".ical", new[] { @"text/calendar" } },
        { @".icalendar", new[] { @"text/calendar" } },
        { @".ico", new[] { @"image/x-icon" } },
        { @".ics", new[] { @"text/calendar" } },
        { @".ief", new[] { @"image/ief" } },
        { @".ifb", new[] { @"text/calendar" } },
        { @".iii", new[] { @"application/x-iphone" } },
        { @".inf", new[] { @"application/octet-stream" } },
        { @".ins", new[] { @"application/x-internet-signup" } },
        { @".isp", new[] { @"application/x-internet-signup" } },
        { @".IVF", new[] { @"video/x-ivf" } },
        { @".jar", new[] { @"application/java-archive" } },
        { @".java", new[] { @"application/octet-stream" } },
        { @".jck", new[] { @"application/liquidmotion" } },
        { @".jcz", new[] { @"application/liquidmotion" } },
        { @".jfif", new[] { @"image/pjpeg" } },
        { @".jpb", new[] { @"application/octet-stream" } },
        { @".jpe", new[] { @"image/jpeg" } },
        { @".jpeg", new[] { @"image/jpeg" } },
        { @".jpg", new[] { @"image/jpeg" } },
        { @".js", new[] { @"text/javascript" } },
        { @".json", new[] { @"application/json" } },
        { @".jsx", new[] { @"text/jscript" } },
        { @".latex", new[] { @"application/x-latex" } },
        { @".lit", new[] { @"application/x-ms-reader" } },
        { @".lpk", new[] { @"application/octet-stream" } },
        { @".lsf", new[] { @"video/x-la-asf" } },
        { @".lsx", new[] { @"video/x-la-asf" } },
        { @".lzh", new[] { @"application/octet-stream" } },
        { @".m13", new[] { @"application/x-msmediaview" } },
        { @".m14", new[] { @"application/x-msmediaview" } },
        { @".m1v", new[] { @"video/mpeg" } },
        { @".m2ts", new[] { @"video/vnd.dlna.mpeg-tts" } },
        { @".m3u", new[] { @"audio/x-mpegurl" } },
        { @".m4a", new[] { @"audio/mp4" } },
        { @".m4v", new[] { @"video/mp4" } },
        { @".man", new[] { @"application/x-troff-man" } },
        { @".manifest", new[] { @"application/x-ms-manifest" } },
        { @".map", new[] { @"text/plain" } },
        { @".markdown", new[] { @"text/markdown" } },
        { @".md", new[] { @"text/markdown" } },
        { @".mdb", new[] { @"application/x-msaccess" } },
        { @".mdp", new[] { @"application/octet-stream" } },
        { @".me", new[] { @"application/x-troff-me" } },
        { @".mht", new[] { @"message/rfc822" } },
        { @".mhtml", new[] { @"message/rfc822" } },
        { @".mid", new[] { @"audio/mid" } },
        { @".midi", new[] { @"audio/mid" } },
        { @".mix", new[] { @"application/octet-stream" } },
        { @".mjs", new[] { @"text/javascript" } },
        { @".mmf", new[] { @"application/x-smaf" } },
        { @".mno", new[] { @"text/xml" } },
        { @".mny", new[] { @"application/x-msmoney" } },
        { @".mov", new[] { @"video/quicktime" } },
        { @".movie", new[] { @"video/x-sgi-movie" } },
        { @".mp2", new[] { @"video/mpeg" } },
        { @".mp3", new[] { @"audio/mpeg" } },
        { @".mp4", new[] { @"video/mp4" } },
        { @".mp4v", new[] { @"video/mp4" } },
        { @".mpa", new[] { @"video/mpeg" } },
        { @".mpe", new[] { @"video/mpeg" } },
        { @".mpeg", new[] { @"video/mpeg" } },
        { @".mpg", new[] { @"video/mpeg" } },
        { @".mpp", new[] { @"application/vnd.ms-project" } },
        { @".mpv2", new[] { @"video/mpeg" } },
        { @".ms", new[] { @"application/x-troff-ms" } },
        { @".msi", new[] { @"application/octet-stream" } },
        { @".mso", new[] { @"application/octet-stream" } },
        { @".mvb", new[] { @"application/x-msmediaview" } },
        { @".mvc", new[] { @"application/x-miva-compiled" } },
        { @".nc", new[] { @"application/x-netcdf" } },
        { @".nsc", new[] { @"video/x-ms-asf" } },
        { @".nws", new[] { @"message/rfc822" } },
        { @".ocx", new[] { @"application/octet-stream" } },
        { @".oda", new[] { @"application/oda" } },
        { @".odc", new[] { @"text/x-ms-odc" } },
        { @".ods", new[] { @"application/oleobject" } },
        { @".oga", new[] { @"audio/ogg" } },
        { @".ogg", new[] { @"video/ogg" } },
        { @".ogv", new[] { @"video/ogg" } },
        { @".ogx", new[] { @"application/ogg" } },
        { @".one", new[] { @"application/onenote" } },
        { @".onea", new[] { @"application/onenote" } },
        { @".onetoc", new[] { @"application/onenote" } },
        { @".onetoc2", new[] { @"application/onenote" } },
        { @".onetmp", new[] { @"application/onenote" } },
        { @".onepkg", new[] { @"application/onenote" } },
        { @".osdx", new[] { @"application/opensearchdescription+xml" } },
        { @".otf", new[] { @"font/otf" } },
        { @".p10", new[] { @"application/pkcs10" } },
        { @".p12", new[] { @"application/x-pkcs12" } },
        { @".p7b", new[] { @"application/x-pkcs7-certificates" } },
        { @".p7c", new[] { @"application/pkcs7-mime" } },
        { @".p7m", new[] { @"application/pkcs7-mime" } },
        { @".p7r", new[] { @"application/x-pkcs7-certreqresp" } },
        { @".p7s", new[] { @"application/pkcs7-signature" } },
        { @".pbm", new[] { @"image/x-portable-bitmap" } },
        { @".pcx", new[] { @"application/octet-stream" } },
        { @".pcz", new[] { @"application/octet-stream" } },
        { @".pdf", new[] { @"application/pdf" } },
        { @".pfb", new[] { @"application/octet-stream" } },
        { @".pfm", new[] { @"application/octet-stream" } },
        { @".pfx", new[] { @"application/x-pkcs12" } },
        { @".pgm", new[] { @"image/x-portable-graymap" } },
        { @".pko", new[] { @"application/vnd.ms-pki.pko" } },
        { @".pma", new[] { @"application/x-perfmon" } },
        { @".pmc", new[] { @"application/x-perfmon" } },
        { @".pml", new[] { @"application/x-perfmon" } },
        { @".pmr", new[] { @"application/x-perfmon" } },
        { @".pmw", new[] { @"application/x-perfmon" } },
        { @".png", new[] { @"image/png" } },
        { @".pnm", new[] { @"image/x-portable-anymap" } },
        { @".pnz", new[] { @"image/png" } },
        { @".pot", new[] { @"application/vnd.ms-powerpoint" } },
        { @".potm", new[] { @"application/vnd.ms-powerpoint.template.macroEnabled.12" } },
        { @".potx", new[] { @"application/vnd.openxmlformats-officedocument.presentationml.template" } },
        { @".ppam", new[] { @"application/vnd.ms-powerpoint.addin.macroEnabled.12" } },
        { @".ppm", new[] { @"image/x-portable-pixmap" } },
        { @".pps", new[] { @"application/vnd.ms-powerpoint" } },
        { @".ppsm", new[] { @"application/vnd.ms-powerpoint.slideshow.macroEnabled.12" } },
        { @".ppsx", new[] { @"application/vnd.openxmlformats-officedocument.presentationml.slideshow" } },
        { @".ppt", new[] { @"application/vnd.ms-powerpoint" } },
        { @".pptm", new[] { @"application/vnd.ms-powerpoint.presentation.macroEnabled.12" } },
        { @".pptx", new[] { @"application/vnd.openxmlformats-officedocument.presentationml.presentation" } },
        { @".prf", new[] { @"application/pics-rules" } },
        { @".prm", new[] { @"application/octet-stream" } },
        { @".prx", new[] { @"application/octet-stream" } },
        { @".ps", new[] { @"application/postscript" } },
        { @".psd", new[] { @"application/octet-stream" } },
        { @".psm", new[] { @"application/octet-stream" } },
        { @".psp", new[] { @"application/octet-stream" } },
        { @".pub", new[] { @"application/x-mspublisher" } },
        { @".qt", new[] { @"video/quicktime" } },
        { @".qtl", new[] { @"application/x-quicktimeplayer" } },
        { @".qxd", new[] { @"application/octet-stream" } },
        { @".ra", new[] { @"audio/x-pn-realaudio" } },
        { @".ram", new[] { @"audio/x-pn-realaudio" } },
        { @".rar", new[] { @"application/octet-stream" } },
        { @".ras", new[] { @"image/x-cmu-raster" } },
        { @".rf", new[] { @"image/vnd.rn-realflash" } },
        { @".rgb", new[] { @"image/x-rgb" } },
        { @".rm", new[] { @"application/vnd.rn-realmedia" } },
        { @".rmi", new[] { @"audio/mid" } },
        { @".roff", new[] { @"application/x-troff" } },
        { @".rpm", new[] { @"audio/x-pn-realaudio-plugin" } },
        { @".rtf", new[] { @"application/rtf" } },
        { @".rtx", new[] { @"text/richtext" } },
        { @".scd", new[] { @"application/x-msschedule" } },
        { @".sct", new[] { @"text/scriptlet" } },
        { @".sea", new[] { @"application/octet-stream" } },
        { @".setpay", new[] { @"application/set-payment-initiation" } },
        { @".setreg", new[] { @"application/set-registration-initiation" } },
        { @".sgml", new[] { @"text/sgml" } },
        { @".sh", new[] { @"application/x-sh" } },
        { @".shar", new[] { @"application/x-shar" } },
        { @".sit", new[] { @"application/x-stuffit" } },
        { @".sldm", new[] { @"application/vnd.ms-powerpoint.slide.macroEnabled.12" } },
        { @".sldx", new[] { @"application/vnd.openxmlformats-officedocument.presentationml.slide" } },
        { @".smd", new[] { @"audio/x-smd" } },
        { @".smi", new[] { @"application/octet-stream" } },
        { @".smx", new[] { @"audio/x-smd" } },
        { @".smz", new[] { @"audio/x-smd" } },
        { @".snd", new[] { @"audio/basic" } },
        { @".snp", new[] { @"application/octet-stream" } },
        { @".spc", new[] { @"application/x-pkcs7-certificates" } },
        { @".spl", new[] { @"application/futuresplash" } },
        { @".spx", new[] { @"audio/ogg" } },
        { @".src", new[] { @"application/x-wais-source" } },
        { @".ssm", new[] { @"application/streamingmedia" } },
        { @".sst", new[] { @"application/vnd.ms-pki.certstore" } },
        { @".stl", new[] { @"application/vnd.ms-pki.stl" } },
        { @".sv4cpio", new[] { @"application/x-sv4cpio" } },
        { @".sv4crc", new[] { @"application/x-sv4crc" } },
        { @".svg", new[] { @"image/svg+xml" } },
        { @".svgz", new[] { @"image/svg+xml" } },
        { @".swf", new[] { @"application/x-shockwave-flash" } },
        { @".t", new[] { @"application/x-troff" } },
        { @".tar", new[] { @"application/x-tar" } },
        { @".tcl", new[] { @"application/x-tcl" } },
        { @".tex", new[] { @"application/x-tex" } },
        { @".texi", new[] { @"application/x-texinfo" } },
        { @".texinfo", new[] { @"application/x-texinfo" } },
        { @".tgz", new[] { @"application/x-compressed" } },
        { @".thmx", new[] { @"application/vnd.ms-officetheme" } },
        { @".thn", new[] { @"application/octet-stream" } },
        { @".tif", new[] { @"image/tiff" } },
        { @".tiff", new[] { @"image/tiff" } },
        { @".toc", new[] { @"application/octet-stream" } },
        { @".tr", new[] { @"application/x-troff" } },
        { @".trm", new[] { @"application/x-msterminal" } },
        { @".ts", new[] { @"video/vnd.dlna.mpeg-tts" } },
        { @".tsv", new[] { @"text/tab-separated-values" } },
        { @".ttc", new[] { @"application/x-font-ttf" } },
        { @".ttf", new[] { @"application/x-font-ttf" } },
        { @".tts", new[] { @"video/vnd.dlna.mpeg-tts" } },
        { @".txt", new[] { @"text/plain" } },
        { @".u32", new[] { @"application/octet-stream" } },
        { @".uls", new[] { @"text/iuls" } },
        { @".ustar", new[] { @"application/x-ustar" } },
        { @".vbs", new[] { @"text/vbscript" } },
        { @".vcf", new[] { @"text/x-vcard" } },
        { @".vcs", new[] { @"text/plain" } },
        { @".vdx", new[] { @"application/vnd.ms-visio.viewer" } },
        { @".vml", new[] { @"text/xml" } },
        { @".vsd", new[] { @"application/vnd.visio" } },
        { @".vss", new[] { @"application/vnd.visio" } },
        { @".vst", new[] { @"application/vnd.visio" } },
        { @".vsto", new[] { @"application/x-ms-vsto" } },
        { @".vsw", new[] { @"application/vnd.visio" } },
        { @".vsx", new[] { @"application/vnd.visio" } },
        { @".vtx", new[] { @"application/vnd.visio" } },
        { @".wasm", new[] { @"application/wasm" } },
        { @".wav", new[] { @"audio/wav" } },
        { @".wax", new[] { @"audio/x-ms-wax" } },
        { @".wbmp", new[] { @"image/vnd.wap.wbmp" } },
        { @".wcm", new[] { @"application/vnd.ms-works" } },
        { @".wdb", new[] { @"application/vnd.ms-works" } },
        { @".webm", new[] { @"video/webm" } },
        { @".webmanifest", new[] { @"application/manifest+json" } },
        { @".webp", new[] { @"image/webp" } },
        { @".wks", new[] { @"application/vnd.ms-works" } },
        { @".wm", new[] { @"video/x-ms-wm" } },
        { @".wma", new[] { @"audio/x-ms-wma" } },
        { @".wmd", new[] { @"application/x-ms-wmd" } },
        { @".wmf", new[] { @"application/x-msmetafile" } },
        { @".wml", new[] { @"text/vnd.wap.wml" } },
        { @".wmlc", new[] { @"application/vnd.wap.wmlc" } },
        { @".wmls", new[] { @"text/vnd.wap.wmlscript" } },
        { @".wmlsc", new[] { @"application/vnd.wap.wmlscriptc" } },
        { @".wmp", new[] { @"video/x-ms-wmp" } },
        { @".wmv", new[] { @"video/x-ms-wmv" } },
        { @".wmx", new[] { @"video/x-ms-wmx" } },
        { @".wmz", new[] { @"application/x-ms-wmz" } },
        { @".woff", new[] { @"application/font-woff" } },
        { @".woff2", new[] { @"font/woff2" } },
        { @".wps", new[] { @"application/vnd.ms-works" } },
        { @".wri", new[] { @"application/x-mswrite" } },
        { @".wrl", new[] { @"x-world/x-vrml" } },
        { @".wrz", new[] { @"x-world/x-vrml" } },
        { @".wsdl", new[] { @"text/xml" } },
        { @".wtv", new[] { @"video/x-ms-wtv" } },
        { @".wvx", new[] { @"video/x-ms-wvx" } },
        { @".x", new[] { @"application/directx" } },
        { @".xaf", new[] { @"x-world/x-vrml" } },
        { @".xaml", new[] { @"application/xaml+xml" } },
        { @".xap", new[] { @"application/x-silverlight-app" } },
        { @".xbap", new[] { @"application/x-ms-xbap" } },
        { @".xbm", new[] { @"image/x-xbitmap" } },
        { @".xdr", new[] { @"text/plain" } },
        { @".xht", new[] { @"application/xhtml+xml" } },
        { @".xhtml", new[] { @"application/xhtml+xml" } },
        { @".xla", new[] { @"application/vnd.ms-excel" } },
        { @".xlam", new[] { @"application/vnd.ms-excel.addin.macroEnabled.12" } },
        { @".xlc", new[] { @"application/vnd.ms-excel" } },
        { @".xlm", new[] { @"application/vnd.ms-excel" } },
        { @".xls", new[] { @"application/vnd.ms-excel" } },
        { @".xlsb", new[] { @"application/vnd.ms-excel.sheet.binary.macroEnabled.12" } },
        { @".xlsm", new[] { @"application/vnd.ms-excel.sheet.macroEnabled.12" } },
        { @".xlsx", new[] { @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
        { @".xlt", new[] { @"application/vnd.ms-excel" } },
        { @".xltm", new[] { @"application/vnd.ms-excel.template.macroEnabled.12" } },
        { @".xltx", new[] { @"application/vnd.openxmlformats-officedocument.spreadsheetml.template" } },
        { @".xlw", new[] { @"application/vnd.ms-excel" } },
        { @".xml", new[] { @"text/xml" } },
        { @".xof", new[] { @"x-world/x-vrml" } },
        { @".xpm", new[] { @"image/x-xpixmap" } },
        { @".xps", new[] { @"application/vnd.ms-xpsdocument" } },
        { @".xsd", new[] { @"text/xml" } },
        { @".xsf", new[] { @"text/xml" } },
        { @".xsl", new[] { @"text/xml" } },
        { @".xslt", new[] { @"text/xml" } },
        { @".xsn", new[] { @"application/octet-stream" } },
        { @".xtp", new[] { @"application/octet-stream" } },
        { @".xwd", new[] { @"image/x-xwindowdump" } },
        { @".z", new[] { @"application/x-compress" } },
        { @".zip", new[] { @"application/zip", @"application/zip-compressed", @"application/x-zip-compressed" } },
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaTypeFileExtensionMapper"/> class.
    /// </summary>
    /// <remarks>
    /// An instance of <see cref="MediaTypeFileExtensionMapper"/> from this constructor provides mappings defined in <see cref="DefaultMap"/>.
    /// </remarks>
    public MediaTypeFileExtensionMapper()
    {
        Mappings = DefaultMap;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaTypeFileExtensionMapper"/> class using a custom mapping between extensions and media types.
    /// </summary>
    /// <param name="map">The custom mapping between extensions and media types.</param>
    public MediaTypeFileExtensionMapper(IDictionary<string, IEnumerable<string>> map) : this(map, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaTypeFileExtensionMapper"/> class using a custom mapping between extensions and media types, and
    /// optionally merge with mapping values from <see cref="DefaultMap"/>.
    /// </summary>
    /// <param name="map">The custom mapping between extensions and media types.</param>
    /// <param name="mergeWithDefaultMappings">
    /// A flag to indicate whether the custom mapping should be merged or not with mapping values from <see cref="DefaultMap"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">If <paramref name="map"/> is <see langword="null"/>.</exception>
    public MediaTypeFileExtensionMapper(IDictionary<string, IEnumerable<string>> map, bool mergeWithDefaultMappings)
    {
        Guard.IsNotNull(map);

        // Create a "copy" of the dictionary to prevent modifications on the input parameter.
        var aux = new Dictionary<string, IEnumerable<string>>(map);

        if (mergeWithDefaultMappings)
        {
            aux.Merge(DefaultMap);
        }

        Mappings = new ReadOnlyDictionary<string, IEnumerable<string>>(aux);
    }

    /// <summary>
    /// Gets the current mappings between extensions and media types.
    /// </summary>
    public IReadOnlyDictionary<string, IEnumerable<string>> Mappings { get; }

    /// <summary>
    /// Gets the media types mapped to the given extension.
    /// </summary>
    /// <param name="extension">The file extension.</param>
    /// <returns>
    /// The media types mapped to the given extension, or n empty value if there are no media types mapped to the given extension.
    /// </returns>
    public IEnumerable<string> GetMediaTypesFromExtension(string extension)
    {
        return Mappings.TryGetValue(extension, out var mediaTypes) ? mediaTypes : Enumerable.Empty<string>();
    }

    /// <summary>
    /// Gets the extensions mapped to the given media type.
    /// </summary>
    /// <param name="mediaType">The media type.</param>
    /// <returns>
    /// The extensions mapped to the given media type, or n empty value if there are no extensions mapped to the given media type.
    /// </returns>
    public IEnumerable<string> GetExtensionsFromMediaType(string mediaType)
    {
        return Mappings.Where(item => item.Value.Contains(mediaType, StringComparer.OrdinalIgnoreCase))
                       .Select(item => item.Key);
    }
}
