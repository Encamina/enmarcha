using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Encamina.Enmarcha.AspNet.Mvc.Bindings;

/// <summary>
/// Binds the incoming form in the request to a dictionary.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
public class FormToDictionaryModelBinder<TKey, TValue> : IModelBinder
{
    /// <inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        if (bindingContext.HttpContext.Request.HasFormContentType)
        {
            var form = bindingContext.HttpContext.Request.Form;

            if (form.ContainsKey(bindingContext.FieldName))
            {
                bindingContext.Result = ModelBindingResult.Success(JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(form[bindingContext.FieldName]));
            }
        }

        return Task.CompletedTask;
    }
}
