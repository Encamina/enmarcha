using System.Globalization;

using Bogus;

namespace Encamina.Enmarcha.Testing;

/// <summary>
/// Provides a faker component that replace external dependencies or values in-place to run tests with an expect or particular outcome.
/// </summary>
/// <remarks>This faker provided is powered by <see href="https://github.com/bchavez/Bogus">Bogus</see>.</remarks>
public sealed class FakerProvider
{
    private readonly IDictionary<CultureInfo, Faker> fakers;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakerProvider"/> class.
    /// </summary>
    public FakerProvider()
    {
        fakers = new Dictionary<CultureInfo, Faker>();
    }

    /// <summary>
    /// Gets a <see cref="Faker"/> by culture.
    /// </summary>
    /// <param name="culture">
    /// The culture from where to get the locale (<see cref="CultureInfo.TwoLetterISOLanguageName"/>) for the <see cref="Faker"/>.
    /// </param>
    /// <returns>A <see cref="Faker"/> for a given culture.</returns>
    public Faker this[CultureInfo culture]
    {
        get
        {
            if (!fakers.TryGetValue(culture, out var faker))
            {
                faker = new Faker(culture.TwoLetterISOLanguageName);
                fakers[culture] = faker;
            }

            return faker;
        }
    }

    /// <summary>
    /// Gets a <see cref="Faker"/>.
    /// </summary>
    /// <remarks>
    /// The returned <see cref="Faker"/> will use – for compatibility – the english neutral as locale.
    /// </remarks>
    /// <returns>A <see cref="Faker"/>.</returns>
    public Faker GetFaker() => GetFaker(CultureInfo.GetCultureInfo(@"en"));

    /// <summary>
    /// Gets a <see cref="Faker"/> for a given culture (which is used to determine the locale for the <see cref="Faker"/>).
    /// </summary>
    /// <param name="culture">
    /// The culture from where to get the locale (<see cref="CultureInfo.TwoLetterISOLanguageName"/>) for the <see cref="Faker"/>.
    /// </param>
    /// <returns>A <see cref="Faker"/> for the given culture.</returns>
    public Faker GetFaker(CultureInfo culture)
    {
        return this[culture];
    }

    /// <summary>
    /// Gets a specific fake object for <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// The returned <see cref="Faker{T}"/> will use – for compatibility – the english neutral as locale.
    /// </remarks>
    /// <typeparam name="T">The type of the object to fake.</typeparam>
    /// <returns>A valid <see cref="Faker{T}"/>.</returns>
    public Faker<T> GetFakerFor<T>() where T : class
        => GetFakerFor<T>(CultureInfo.GetCultureInfo(@"en"));

    /// <summary>
    /// Gets a specific fake object for <typeparamref name="T"/>, with given
    /// culture (which is used to determine the locale for the <see cref="Faker{T}"/>).
    /// </summary>
    /// <typeparam name="T">The type of the object to fake.</typeparam>
    /// <param name="culture">
    /// The culture from where to get the locale (<see cref="CultureInfo.TwoLetterISOLanguageName"/>) for the <see cref="Faker{T}"/>.
    /// </param>
    /// <returns>A valid <see cref="Faker{T}"/> for the given culture.</returns>
    public Faker<T> GetFakerFor<T>(CultureInfo culture) where T : class
    {
        return new Faker<T>(culture.TwoLetterISOLanguageName);
    }
}
