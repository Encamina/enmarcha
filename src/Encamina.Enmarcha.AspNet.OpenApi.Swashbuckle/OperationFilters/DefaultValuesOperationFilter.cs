using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ApiExplorer;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle.OperationFilters;

/// <summary>
/// An operation filter for <c>Swashbuckle</c> that enriches the generated OpenAPI documents with default values for parameters.
/// </summary>
public sealed class DefaultValuesOperationFilter : IOperationFilter
{
    private const string Default = @"default";

    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            var response = operation.Responses[responseType.IsDefaultResponse ? Default : responseType.StatusCode.ToString()];

            foreach (var contentType in response.Content.Keys)
            {
                if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
                {
                    response.Content.Remove(contentType);
                }
            }
        }

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null && description.ModelMetadata != null)
            {
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType));
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
