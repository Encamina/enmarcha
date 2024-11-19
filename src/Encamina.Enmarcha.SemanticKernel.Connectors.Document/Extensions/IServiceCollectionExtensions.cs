using Encamina.Enmarcha.AI.Abstractions;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up services in a <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentContentExtractor"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentContentExtractor(this IServiceCollection services)
    {
        return services.AddScoped<IDocumentContentExtractor, DefaultDocumentContentExtractor>();
    }

    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentContentExtractor"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentContentExtractor(this IServiceCollection services, Func<string, int> lengthFunction)
    {
        services.AddSingleton(lengthFunction);
        return services.AddDefaultDocumentContentExtractor();
    }

    /// <summary>
    /// Adds a default implementation of Semantic <see cref="IDocumentContentExtractor"/> to the specified <see cref="IServiceCollection"/> as a scoped service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentContentSemanticExtractor(this IServiceCollection services)
    {
        return services.AddDefaultDocumentContentSemanticExtractor(ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a default implementation of Semantic <see cref="IDocumentContentExtractor"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <param name="serviceLifetime">The lifetime for the registered services.</param>
    public static IServiceCollection AddDefaultDocumentContentSemanticExtractor(this IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        return services.AddType<IDocumentContentExtractor, DefaultDocumentContentSemanticExtractor>(serviceLifetime);
    }

    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentConnectorProvider"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentConnectorProvider(this IServiceCollection services)
    {
        return services.AddScoped<IDocumentConnectorProvider, DefaultDocumentContentExtractor>();
    }

    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentConnectorProvider"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentConnectorProvider(this IServiceCollection services, Func<string, int> lengthFunction)
    {
        services.AddSingleton(lengthFunction);
        return services.AddDefaultDocumentConnectorProvider();
    }

    /// <summary>
    /// Adds the <see cref="WordDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddWordDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, WordDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="DocDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDocDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, DocDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="HtmlDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddHtmlDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, HtmlDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="CleanPdfDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCleanPdfDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, CleanPdfDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="BasePptxDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBasePptxDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, BasePptxDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="ExcelToMarkdownDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddExcelToMarkdownDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, ExcelToMarkdownDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="ParagraphPptxDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddParagraphPptxDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, ParagraphPptxDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="PdfDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddPdfDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, PdfDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="PdfWithTocDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddPdfWithTocDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, PdfWithTocDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="SlidePptxDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSlidePptxDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, SlidePptxDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="StrictFormatCleanPdfDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddStrictFormatCleanPdfDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, StrictFormatCleanPdfDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="TxtDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddTxtDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, TxtDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="VttDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddVttDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, VttDocumentConnector>();
    }

    /// <summary>
    /// Adds the <see cref="CsvTsvDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCsvTsvDocumentConnector(this IServiceCollection services)
    {
        return services.AddSingleton<IEnmarchaDocumentConnector, CsvTsvDocumentConnector>();
    }

    /// <summary>
    /// Adds and configures the <see cref="SkVisionImageDocumentConnector"/> implementation of <see cref="IEnmarchaDocumentConnector"/> to the specified <see cref="IServiceCollection"/> as a scoped service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSkVisionImageDocumentConnector(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SkVisionImageDocumentConnectorOptions>()
                .Bind(configuration.GetSection(nameof(SkVisionImageDocumentConnectorOptions)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        return services.AddScoped<IEnmarchaDocumentConnector, SkVisionImageDocumentConnector>();
    }

    /// <summary>
    /// Adds the default document connectors to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentConnectors(this IServiceCollection services)
    {
        services.AddWordDocumentConnector();
        services.AddCleanPdfDocumentConnector();
        services.AddParagraphPptxDocumentConnector();
        services.AddTxtDocumentConnector();
        services.AddVttDocumentConnector();
        services.AddExcelToMarkdownDocumentConnector();

        return services;
    }
}
