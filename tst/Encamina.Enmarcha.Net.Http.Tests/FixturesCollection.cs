namespace Encamina.Enmarcha.Testing;

/// <summary>
/// <para>
///     Used to decorate test classes and collections to indicate a test which has per-test-collection fixture data.
/// </para>
/// <para>
///     An instance of the fixture data is initialized just before the first test in the collection is run, and if
///     it implements <see cref="IDisposable"/>, it will be disposed after the last test in the collection is run.
/// </para>
/// <para>
///     To gain access to the fixture data from inside the test, a constructor argument should be added to the test
///     class matching exactly matches the <c>TFixture</c> type parameter from any of the <see cref="ICollectionFixture{TFixture}"/>.
///     referenced in this collection.
/// </para>
/// </summary>
/// <remarks>
/// Fixture classes can be shared across assemblies, but fixture collection definitions must be in the same assembly
/// as the test that uses them, othertwise they will not work.
/// </remarks>
[CollectionDefinition(MagicStrings.FixturesCollection)]
public sealed class FixturesCollection
    : ICollectionFixture<FakerProvider>
{
    /*************************************************************************/
    /*  This class has no code, and is never created. Its purpose is simply  */
    /*  to be the place to apply [CollectionDefinition] and all the          */
    /*  ICollectionFixture<> interfaces.                                     */
    /*************************************************************************/
}
