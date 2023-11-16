using System.Globalization;

using CommunityToolkit.Diagnostics;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Encamina.Enmarcha.AspNet.Mvc.Bindings;

/// <summary>
/// Custom model binder for date times.
/// </summary>
internal sealed class CustomDateTimeModelBinder : IModelBinder
{
    /// <summary>
    /// Supported date time types.
    /// </summary>
    internal static readonly Type[] SupportedDateTimeTypes = new[] { typeof(DateTime), typeof(DateTime?) };

    private static readonly string[] SupportedCustomDateFormats = new[]
    {
        @"yyyyMMddTHHmmssZ",
        @"yyyyMMddTHHmmZ",
        @"yyyyMMddTHHmmss",
        @"yyyyMMddTHHmm",
        @"yyyyMMddHHmmss",
        @"yyyyMMddHHmm",
        @"yyyyMMdd",
        @"yyyy-MM-ddTHH-mm-ss",
        @"yyyy-MM-dd-HH-mm-ss",
        @"yyyy-MM-dd-HH-mm",
        @"yyyy-MM-dd",
        @"MM-dd-yyyy",
        @"dd-MM-yyyy",
    };

    private readonly string customDateTimeFormat;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDateTimeModelBinder"/> class with specific custom date time format.
    /// </summary>
    /// <param name="customDateTimeFormat">The custom <see cref="DateTime"/> format to use when binding models.</param>
    public CustomDateTimeModelBinder(string customDateTimeFormat)
    {
        this.customDateTimeFormat = customDateTimeFormat;
    }

    /// <inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        Guard.IsNotNull(bindingContext);

        if (!SupportedDateTimeTypes.Contains(bindingContext.ModelType))
        {
            return Task.CompletedTask;
        }

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var dateTimeToParse = valueProviderResult.FirstValue;

        if (string.IsNullOrWhiteSpace(dateTimeToParse))
        {
            return Task.CompletedTask;
        }

        var formattedDateTime = string.IsNullOrWhiteSpace(customDateTimeFormat)
            ? ParseDateTime(dateTimeToParse)
            : ParseDateTime(dateTimeToParse, new[] { customDateTimeFormat });

        bindingContext.Result = ModelBindingResult.Success(formattedDateTime);

        return Task.CompletedTask;
    }

    private static DateTime? ParseDateTime(string dateToParse, string[] formats = null)
    {
        if (formats == null)
        {
            formats = SupportedCustomDateFormats;
        }

        foreach (var format in formats)
        {
            if (format.EndsWith(@"Z") && DateTime.TryParseExact(dateToParse, format, null, DateTimeStyles.None, out var validDate))
            {
                return validDate;
            }

            if (DateTime.TryParseExact(dateToParse, format, null, DateTimeStyles.None, out validDate))
            {
                return validDate;
            }
        }

        return null;
    }
}
