using System.Text;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Net.Http;

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Encamina.Enmarcha.AspNet.Mvc.Formatters.Csv;

/// <summary>
/// An output formatter that writes a comma separated values files (<c>.csv</c>) to a request body.
/// </summary>
internal class CsvOutputFormatter : OutputFormatter
{
    private readonly CsvFormatterOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvOutputFormatter"/> class.
    /// </summary>
    /// <param name="options">Configuration options for this formatter.</param>
    public CsvOutputFormatter(CsvFormatterOptions options)
    {
        this.options = options;

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Csv));
    }

    /// <inheritdoc/>
    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        return CanWriteType(context?.ObjectType);
    }

    /// <inheritdoc/>
    public async override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var type = context.Object.GetType();
        var itemType = type.GetGenericArguments().Length > 0 ? type.GetGenericArguments()[0] : type.GetElementType();

        var stringBuilder = new StringBuilder();

        if (options.UseHeader)
        {
            stringBuilder.AppendJoin(options.Delimiter, itemType.GetProperties().Select(p => Helpers.GetDisplayName(p)));
        }

        var objects = (IEnumerable<object>)context.Object;

        foreach (var obj in objects)
        {
            ProcessObject(stringBuilder, obj);
        }

        var response = context.HttpContext.Response;

        using var streamWriter = new StreamWriter(response.Body, Encoding.GetEncoding(options.Encoding));
        await streamWriter.WriteAsync(stringBuilder.ToString());
        await streamWriter.FlushAsync();
    }

    /// <inheritdoc/>
    protected override bool CanWriteType(Type type)
    {
        Guard.IsNotNull(type);

        return Helpers.IsTypeOfIEnumerable(type);
    }

    private void ProcessObject(StringBuilder stringBuilder, object obj)
    {
        var values = obj.GetType().GetProperties().Select(propertyInfo => new
        {
            Value = propertyInfo.GetValue(obj, null),
        });

        foreach (var currentValue in values)
        {
            if (currentValue == null)
            {
                stringBuilder.AppendJoin(options.Delimiter, string.Empty);
            }
            else
            {
                var value = currentValue.Value.ToString();

                if (value.Contains(','))
                {
                    value = string.Concat("\"", value, "\"");
                }

                value = value.Replace("\r", " ").Replace("\n", " ");

                stringBuilder.AppendJoin(options.Delimiter, value);
            }
        }
    }
}
