using System.Collections;
using System.Reflection;
using System.Text;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Net.Http;

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Encamina.Enmarcha.AspNet.Mvc.Formatters.Csv;

/// <summary>
/// An input formatter that reads a comma separated values files (<c>.csv</c>) from a request body.
/// </summary>
internal class CsvInputFormatter : InputFormatter
{
    private readonly CsvFormatterOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvInputFormatter"/> class.
    /// </summary>
    /// <param name="options">Configuration options for this formatter.</param>
    public CsvInputFormatter(CsvFormatterOptions options)
    {
        this.options = options;

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Csv));
    }

    /// <inheritdoc/>
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var result = await ReadStreamAsync(context.ModelType, context.HttpContext.Request.Body);

        return await (result == null ? InputFormatterResult.NoValueAsync() : InputFormatterResult.SuccessAsync(result));
    }

    /// <inheritdoc/>
    public override bool CanRead(InputFormatterContext context)
    {
        return CanReadType(context?.ModelType);
    }

    /// <inheritdoc/>
    protected override bool CanReadType(Type type)
    {
        Guard.IsNotNull(type);

        return Helpers.IsTypeOfIEnumerable(type);
    }

    private async Task<object> ReadStreamAsync(Type type, Stream stream)
    {
        IList list;
        Type itemType;
        var typeIsArray = false;

        if (type.GetGenericArguments().Length > 0)
        {
            itemType = type.GetGenericArguments()[0];
            list = (IList)Activator.CreateInstance(type);
        }
        else
        {
            typeIsArray = true;
            itemType = type.GetElementType();

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(itemType);

            list = (IList)Activator.CreateInstance(constructedListType);
        }

        var reader = new StreamReader(stream, Encoding.GetEncoding(options.Encoding));

        var skipFirstLine = options.UseHeader;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            var values = line.Split(options.Delimiter);

            if (skipFirstLine)
            {
                skipFirstLine = false;
            }
            else
            {
                var itemTypeInGeneric = list.GetType().GetTypeInfo().GenericTypeArguments[0];
                var item = Activator.CreateInstance(itemTypeInGeneric);
                var properties = item.GetType().GetProperties();

                for (var i = 0; i < values.Length; i++)
                {
                    properties[i].SetValue(item, Convert.ChangeType(values[i], properties[i].PropertyType), null);
                }

                list.Add(item);
            }
        }

        if (typeIsArray)
        {
            var array = Array.CreateInstance(itemType, list.Count);

            for (var t = 0; t < list.Count; t++)
            {
                array.SetValue(list[t], t);
            }

            return array;
        }

        return list;
    }
}