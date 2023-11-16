using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle.OperationFilters;

/// <summary>
/// An operation filter for <c>Swashbuckle</c> that adds information about an API Key header if required.
/// </summary>
public sealed class ApiKeyHeaderOperationFilter : IOperationFilter
{
    private const string DefaultHeaderApiKey = @"x-api-key";
    private const string DefaultDescription = @"A pre-shared API Key required to access an API.";

    private readonly string headerApiKey;
    private readonly string description;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyHeaderOperationFilter"/> class.
    /// </summary>
    public ApiKeyHeaderOperationFilter() : this(DefaultHeaderApiKey, DefaultDescription)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyHeaderOperationFilter"/> class.
    /// </summary>
    /// <param name="headerApiKey">The name of the header with the API Key.</param>
    public ApiKeyHeaderOperationFilter(string headerApiKey) : this(headerApiKey, DefaultDescription)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyHeaderOperationFilter"/> class.
    /// </summary>
    /// <param name="headerApiKey">The name of the header with the API Key.</param>
    /// <param name="description">A description for the header with the API Key.</param>
    public ApiKeyHeaderOperationFilter(string headerApiKey, string description)
    {
        this.description = string.IsNullOrWhiteSpace(description) ? DefaultDescription : description;
        this.headerApiKey = string.IsNullOrWhiteSpace(headerApiKey) ? DefaultHeaderApiKey : headerApiKey;
    }

    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }

        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = headerApiKey,
            Description = description,
            In = ParameterLocation.Header,
            Schema = new OpenApiSchema() { Type = @"string" },
            Required = true,
        });
    }
}
