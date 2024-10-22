using System.Reflection;

using Encamina.Enmarcha.Core.DataAnnotations;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle.SchemaFilters;

/// <summary>
/// This filter enforces that at least one of a specified set of properties must be required in an OpenAPI schema.
/// It processes types decorated with the <see cref="AtLeastOneRequiredAttribute"/>.
/// </summary>
public sealed class AtLeastOneRequiredSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Applies the filter to modify the OpenAPI schema based on the <see cref="AtLeastOneRequiredAttribute"/>.
    /// It removes individual property requirements and replaces them with an 'anyOf' requirement
    /// for the specified properties, ensuring that at least one of them is present in the request.
    /// </summary>
    /// <param name="schema">The <see cref="OpenApiSchema"/> to modify.</param>
    /// <param name="context">
    /// The <see cref="SchemaFilterContext"/> containing metadata about the context, including the type being processed.
    /// </param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var attribute = context.Type.GetCustomAttribute<AtLeastOneRequiredAttribute>();
        if (attribute != null)
        {
            // Add 'anyOf' to the schema with the specified properties
            schema.AnyOf = attribute.PropertyNames.Select(propertyName => new OpenApiSchema
            {
                Required = new HashSet<string> { propertyName },
            }).ToList();
        }
    }
}
