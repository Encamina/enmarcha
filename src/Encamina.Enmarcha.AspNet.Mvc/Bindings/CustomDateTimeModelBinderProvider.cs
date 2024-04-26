using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Encamina.Enmarcha.AspNet.Mvc.Bindings;

/// <summary>
/// Provides custom <see cref="DateTime"/> model binders as a valid <see cref="IModelBinderProvider"/> that
/// can be registered in <c>MvcOptions</c>.
/// </summary>
public class CustomDateTimeModelBinderProvider : IModelBinderProvider
{
    private readonly string customDateTimeFormat;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDateTimeModelBinderProvider"/> class.
    /// </summary>
    public CustomDateTimeModelBinderProvider() : this(null!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDateTimeModelBinderProvider"/> class with specific custom date time format.
    /// </summary>
    /// <param name="customDateTimeFormat">The custom <see cref="DateTime"/> format to use when binding models.</param>
    public CustomDateTimeModelBinderProvider(string customDateTimeFormat)
    {
        this.customDateTimeFormat = customDateTimeFormat;
    }

    /// <inheritdoc/>
    public virtual IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return CustomDateTimeModelBinder.SupportedDateTimeTypes.Contains(context.Metadata.ModelType)
            ? new CustomDateTimeModelBinder(customDateTimeFormat)
            : (IModelBinder?)null;
    }
}
