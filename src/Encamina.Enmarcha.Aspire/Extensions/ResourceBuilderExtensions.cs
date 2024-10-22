using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

using Encamina.Enmarcha.Aspire.Extensions;

namespace Encamina.Enmarcha.Aspire.Extensions;

/// <summary>
/// Provides extension methods for configuring Aspire resources.
/// </summary>
public static class ResourceBuilderExtensions
{
    /// <summary>
    /// Adds an environment variable array to the resource.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the environment variable.</param>
    /// <param name="values">The array of values of the environment variable.</param>
    /// <returns>A resource configured with the specified environment variable.</returns>
    public static IResourceBuilder<T> WithEnvironment<T>(this IResourceBuilder<T> builder, string name, string[] values) where T : IResourceWithEnvironment
    {
        return builder.WithEnvironment(context =>
        {
            for (var i = 0; i < values.Length; i++)
            {
                context.EnvironmentVariables[$"{name}:{i}"] = values[i];
            }
        });
    }
}
