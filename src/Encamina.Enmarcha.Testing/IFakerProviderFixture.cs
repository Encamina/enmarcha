namespace Encamina.Enmarcha.Testing;

/// <summary>
/// Represents a fixture (for tests) that provides support a faker provider.
/// </summary>
public interface IFakerProviderFixture
{
    /// <summary>
    /// Gets the faker provider.
    /// </summary>
    FakerProvider FakerProvider { get; }
}
