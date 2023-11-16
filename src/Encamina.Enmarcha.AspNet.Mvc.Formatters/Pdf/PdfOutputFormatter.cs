using Encamina.Enmarcha.Net.Http;

using Microsoft.AspNetCore.Mvc.Formatters;

namespace Encamina.Enmarcha.AspNet.Mvc.Formatters.Pdf;

internal class PdfOutputFormatter : OutputFormatter
{
    public PdfOutputFormatter()
    {
        SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Pdf));
    }

    /// <inheritdoc/>
    public async override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        await Task.Run(() => context.HttpContext.Response);
    }

    /// <inheritdoc/>
    protected override bool CanWriteType(Type type) => true;
}
