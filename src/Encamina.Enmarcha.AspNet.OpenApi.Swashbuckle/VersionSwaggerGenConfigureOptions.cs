using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle;

/// <summary>
/// A Swagger generation configuration option for versioned OpenAPI documents.
/// </summary>
internal sealed class VersionSwaggerGenConfigureOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider apiVersionDescriptionProvider;
    private readonly VersionSwaggerGenOptions versionSwaggerGenOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionSwaggerGenConfigureOptions"/> class.
    /// </summary>
    /// <param name="apiVersionDescriptionProvider">A valid instance of <see cref="IApiVersionDescriptionProvider"/> to get version information.</param>
    /// <param name="options">Configuration options for this Swagger Generation configuration options.</param>
    public VersionSwaggerGenConfigureOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider, IOptions<VersionSwaggerGenOptions> options)
    {
        this.apiVersionDescriptionProvider = apiVersionDescriptionProvider;
        versionSwaggerGenOptions = options.Value;
    }

    /// <inheritdoc/>
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            var version = apiVersionDescription.ApiVersion == ApiVersion.Neutral ? string.Empty : apiVersionDescription.ApiVersion.ToString();

            options.SwaggerDoc(apiVersionDescription.GroupName, new OpenApiInfo()
            {
                Title = $@"{versionSwaggerGenOptions.Title} {version}",
                Version = version,
            });
        }
    }
}
