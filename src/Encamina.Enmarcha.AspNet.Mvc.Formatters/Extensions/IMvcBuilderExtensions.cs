using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AspNet.Mvc.Formatters.Csv;
using Encamina.Enmarcha.AspNet.Mvc.Formatters.Pdf;

using Encamina.Enmarcha.Net.Http;

using Microsoft.Net.Http.Headers;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension method to add services and other goodies to an <see cref="IMvcBuilder"/>.
/// </summary>
public static class IMvcBuilderExtensions
{
    private const string FormatCSV = @"csv";
    private const string FormatPDF = @"pdf";

    /// <summary>
    /// Adds an input formatter for comma separated values (<c>.csv</c>).
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvInputFormatter(this IMvcBuilder builder)
    {
        return AddCsvFormatters(builder, null, true, false);
    }

    /// <summary>
    /// Adds an input formatter for comma separated values (<c>.csv</c>) with given options.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name="options">An action to configure options for the input formatter.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvInputFormatter(this IMvcBuilder builder, Action<CsvFormatterOptions> options)
    {
        var csvFormatterOptions = new CsvFormatterOptions();
        options.Invoke(csvFormatterOptions);

        return AddCsvFormatters(builder, csvFormatterOptions, true, false);
    }

    /// <summary>
    /// Adds an input formatter for comma separated values (<c>.csv</c>) with given options.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name="csvFormatterOptions">Configuration options for the input formatter.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvInputFormatter(this IMvcBuilder builder, CsvFormatterOptions csvFormatterOptions)
    {
        return AddCsvFormatters(builder, csvFormatterOptions, true, false);
    }

    /// <summary>
    /// Adds an output formatter for comma separated values (<c>.csv</c>).
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvOutputFormatter(this IMvcBuilder builder)
    {
        return AddCsvFormatters(builder, null, false, true);
    }

    /// <summary>
    /// Adds an output formatter for comma separated values (<c>.csv</c>) with given options.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name="options">An action to configure options for the output formatter.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvOutputFormatter(this IMvcBuilder builder, Action<CsvFormatterOptions> options)
    {
        var csvFormatterOptions = new CsvFormatterOptions();
        options.Invoke(csvFormatterOptions);

        return AddCsvFormatters(builder, csvFormatterOptions, false, true);
    }

    /// <summary>
    /// Adds an output formatter for comma separated values (<c>.csv</c>) with given options.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name="csvFormatterOptions">Configuration options for the output formatter.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvOutputFormatter(this IMvcBuilder builder, CsvFormatterOptions csvFormatterOptions)
    {
        return AddCsvFormatters(builder, csvFormatterOptions, false, true);
    }

    /// <summary>
    /// Adds boths input and output formatters for comma separated values (<c>.csv</c>).
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvFormatters(this IMvcBuilder builder)
    {
        return AddCsvFormatters(builder, null, true, true);
    }

    /// <summary>
    /// Adds boths input and output formatters for comma separated values (<c>.csv</c>) with given options.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name="options">An action to configure options for the input and output formatters.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvFormatters(this IMvcBuilder builder, Action<CsvFormatterOptions> options)
    {
        var csvFormatterOptions = new CsvFormatterOptions();
        options.Invoke(csvFormatterOptions);

        return AddCsvFormatters(builder, csvFormatterOptions, true, true);
    }

    /// <summary>
    /// Adds boths input and output formatters for comma separated values (<c>.csv</c>) with given options.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name="csvFormatterOptions">Configuration options for the input and output formatters.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddCsvFormatters(this IMvcBuilder builder, CsvFormatterOptions csvFormatterOptions)
    {
        return AddCsvFormatters(builder, csvFormatterOptions, true, true);
    }

    /// <summary>
    /// Adds an output formatter for portable document format (<c>.pdf</c>) files.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure additional the MVC services or chain additional calls.</returns>
    public static IMvcBuilder AddPdfOutputFormatter(this IMvcBuilder builder)
    {
        Guard.IsNotNull(builder);

        builder.AddFormatterMappings(m => m.SetMediaTypeMappingForFormat(FormatPDF, new MediaTypeHeaderValue(MediaTypeNames.Application.Pdf)));

        builder.AddMvcOptions(options => options.OutputFormatters.Add(new PdfOutputFormatter()));

        return builder;
    }

    private static IMvcBuilder AddCsvFormatters(IMvcBuilder builder, CsvFormatterOptions csvFormatterOptions, bool addInputFormater, bool addOutputFormater)
    {
        Guard.IsNotNull(builder);

        if (csvFormatterOptions == null)
        {
            csvFormatterOptions = new CsvFormatterOptions();
        }
        else
        {
            Guard.IsNotNull(csvFormatterOptions.Encoding);
        }

        builder.AddFormatterMappings(mapping => mapping.SetMediaTypeMappingForFormat(FormatCSV, new MediaTypeHeaderValue(MediaTypeNames.Text.Csv)));

        if (addInputFormater)
        {
            builder.AddMvcOptions(options => options.InputFormatters.Add(new CsvInputFormatter(csvFormatterOptions)));
        }

        if (addOutputFormater)
        {
            builder.AddMvcOptions(options => options.OutputFormatters.Add(new CsvOutputFormatter(csvFormatterOptions)));
        }

        return builder;
    }
}
