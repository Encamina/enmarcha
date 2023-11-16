using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.Testing;

namespace Encamina.Enmarcha.Net.Http.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class MediaTypeFileExtensionMapperTests : FakerProviderFixturedBase
{
    public MediaTypeFileExtensionMapperTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void MediaTypeFileExtensionMapper_WithNullMap_ThrowsException()
    {
        // Act...
        var exception = Record.Exception(() => new MediaTypeFileExtensionMapper(null));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"map", ((ArgumentNullException)exception).ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MediaTypeFileExtensionMapper_WithNullMap_And_MergeWithDefaultMappings_ThrowsException(bool mergeWithDefaultMappings)
    {
        // Act...
        var exception = Record.Exception(() => new MediaTypeFileExtensionMapper(null, mergeWithDefaultMappings));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"map", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void MediaTypeFileExtensionMapper_WithDefaultMappings_Succeeds()
    {
        // Act...
        var mapper = new MediaTypeFileExtensionMapper();

        // Assert...
        Assert.NotNull(mapper);
        Assert.NotNull(mapper.Mappings);
        Assert.NotEmpty(mapper.Mappings);
        Assert.Equal(MediaTypeFileExtensionMapper.DefaultMap, mapper.Mappings);
    }

    [Fact]
    public void MediaTypeFileExtensionMapper_WithEmptyDictionary_And_MergeWithDefaultMappings_Succeeds()
    {
        // Arrange...
        var emptyDictionary = new Dictionary<string, IEnumerable<string>>();

        // Act...
        var mapper = new MediaTypeFileExtensionMapper(emptyDictionary, true);

        // Assert...
        Assert.NotNull(mapper);
        Assert.NotNull(mapper.Mappings);
        Assert.NotEmpty(mapper.Mappings);
        Assert.Equal(MediaTypeFileExtensionMapper.DefaultMap, mapper.Mappings);
    }

    [Fact]
    public void MediaTypeFileExtensionMapper_WithEmptyDictionary_And_No_MergeWithDefaultMappings_Succeeds()
    {
        // Arrange...
        var emptyDictionary = new Dictionary<string, IEnumerable<string>>();

        // Act...
        var mapper = new MediaTypeFileExtensionMapper(emptyDictionary, false);

        // Assert...
        Assert.NotNull(mapper);
        Assert.NotNull(mapper.Mappings);
        Assert.Empty(mapper.Mappings);
        Assert.NotEqual(MediaTypeFileExtensionMapper.DefaultMap, mapper.Mappings);
    }

    [Fact]
    public void MediaTypeFileExtensionMapper_WithDictionary_And_MergeWithDefaultMappings_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var dictionary = new Dictionary<string, IEnumerable<string>>()
        {
            { faker.Random.Word(), new[] { faker.Internet.Random.Word() } },
        };

        // Act...
        var mapper = new MediaTypeFileExtensionMapper(dictionary, true);

        // Assert...
        Assert.NotNull(mapper);
        Assert.NotNull(mapper.Mappings);
        Assert.NotEmpty(mapper.Mappings);

        dictionary.Merge(mapper.Mappings);
        Assert.Equal(dictionary, mapper.Mappings);
    }

    [Fact]
    public void MediaTypeFileExtensionMapper_WithDictionary_And_No_MergeWithDefaultMappings_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var dictionary = new Dictionary<string, IEnumerable<string>>()
        {
            { faker.Random.Word(), new[] { faker.Internet.Random.Word() } },
        };

        // Act...
        var mapper = new MediaTypeFileExtensionMapper(dictionary, false);

        // Assert...
        Assert.NotNull(mapper);
        Assert.NotNull(mapper.Mappings);
        Assert.NotEmpty(mapper.Mappings);
        Assert.Equal(dictionary, mapper.Mappings);
    }

    [Fact]
    public void MediaTypeFileExtensionMapper_GetMediaTypesFromExtension_WithNull_ThrowsException()
    {
        // Arrange...
        var mapper = new MediaTypeFileExtensionMapper();

        // Act...
        var exception = Record.Exception(() => mapper.GetMediaTypesFromExtension(null));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"key", ((ArgumentNullException)exception).ParamName);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    public void MediaTypeFileExtensionMapper_GetMediaTypesFromExtension_WithStringEmpty_Succeeds(string value)
    {
        // Arrange...
        var mapper = new MediaTypeFileExtensionMapper();

        // Act...
        var mediaTypes = mapper.GetMediaTypesFromExtension(value);

        // Assert...
        Assert.NotNull(mediaTypes);
        Assert.Empty(mediaTypes);
    }

    [Fact]
    public void MediaTypeFileExtensionMapper_GetMediaTypesFromExtension_WithValueFromDefaultMappings_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var mapper = new MediaTypeFileExtensionMapper();
        var item = faker.PickRandom(mapper.Mappings.ToArray());

        // Act...
        var mediaTypes = mapper.GetMediaTypesFromExtension(item.Key);

        // Assert...
        Assert.NotNull(mediaTypes);
        Assert.NotEmpty(mediaTypes);
        Assert.Equal(item.Value, mediaTypes);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MediaTypeFileExtensionMapper_GetMediaTypesFromExtension_WithCustomValue_WithOrWithoutDefaultMappings_Succeeds(bool mergeWithDefault)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var testExtension = faker.Random.Word();
        var testMediaType = new[] { faker.Internet.Random.Word() };
        var dictionary = new Dictionary<string, IEnumerable<string>>()
        {
            { testExtension, testMediaType },
        };
        var mapper = new MediaTypeFileExtensionMapper(dictionary, mergeWithDefault);

        // Act...
        var mediaTypes = mapper.GetMediaTypesFromExtension(testExtension);

        // Assert...
        Assert.NotNull(mediaTypes);
        Assert.NotEmpty(mediaTypes);
        Assert.Equal(testMediaType, mediaTypes);
    }
}