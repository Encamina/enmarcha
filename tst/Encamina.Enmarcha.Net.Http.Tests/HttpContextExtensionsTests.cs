using Encamina.Enmarcha.Net.Http.Extensions;
using Encamina.Enmarcha.Testing;

using Microsoft.AspNetCore.Http;

using Moq;

namespace Encamina.Enmarcha.Net.Http.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class HttpContextExtensionsTests : FakerProviderFixturedBase
{
    public HttpContextExtensionsTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void ReadValueFromRequestHeader_NullHttpContext_ThrowsException()
    {
        // Arrange...
        HttpContext httpContext = null;

        // Act...
        var exception = Record.Exception(() => httpContext.ReadValueFromRequestHeader(It.IsAny<string>(), It.IsAny<string>()));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"httpContext", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void ReadValueFromRequestHeader_NullHeaderName_ThrowsException()
    {
        // Arrange...
        var httpContext = new Mock<HttpContext>().Object;

        // Act...
        var exception = Record.Exception(() => httpContext.ReadValueFromRequestHeader(null, It.IsAny<string>()));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"headerName", ((ArgumentNullException)exception).ParamName);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    public void ReadValueFromRequestHeader_EmptyOrWitheSpaceHeaderName_Succeeds(string headerName)
    {
        // Arrange...
        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpRequest = new Mock<HttpRequest>();
        var mockHeaders = new Mock<IHeaderDictionary>();

        var faker = FakerProvider.GetFaker();

        mockHeaders.Setup(h => h[faker.Internet.Random.Word()]).Returns(faker.Internet.Random.Word());
        mockHttpRequest.Setup(r => r.Headers).Returns(mockHeaders.Object);
        mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);

        // Act...
        var result = mockHttpContext.Object.ReadValueFromRequestHeader(headerName, It.IsAny<string>());

        // Assert...
        Assert.Null(result);
    }

    [Fact]
    public void ReadValueFromRequestHeader_WithHeaderName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var fakeHeader = faker.Internet.Random.Word();
        var fakerHeaderValue = new Microsoft.Extensions.Primitives.StringValues(faker.Internet.Random.Word());

        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpRequest = new Mock<HttpRequest>();
        var mockHeaders = new Mock<IHeaderDictionary>();

        mockHeaders.Setup(r => r.TryGetValue(fakeHeader, out fakerHeaderValue)).Returns(true);
        mockHttpRequest.Setup(r => r.Headers).Returns(mockHeaders.Object);
        mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);

        // Act...
        var result = mockHttpContext.Object.ReadValueFromRequestHeader(fakeHeader, It.IsAny<string>());

        // Assert...
        Assert.NotNull(result);
        Assert.Equal(fakerHeaderValue, result);
    }

    [Fact]
    public void ReadValueFromRequestHeader_WithHeaderName_HeaderNotFoud_NullDefault_Succeeds()
    {
        // Arrange...
        var anyHeaderValue = It.IsAny<Microsoft.Extensions.Primitives.StringValues>();

        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpRequest = new Mock<HttpRequest>();
        var mockHeaders = new Mock<IHeaderDictionary>();

        mockHeaders.Setup(r => r.TryGetValue(It.IsAny<string>(), out anyHeaderValue)).Returns(false);
        mockHttpRequest.Setup(r => r.Headers).Returns(mockHeaders.Object);
        mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);

        // Act...
        var result = mockHttpContext.Object.ReadValueFromRequestHeader(FakerProvider.GetFaker().Internet.Random.Word(), null);

        // Assert...
        Assert.Null(result);
    }

    [Fact]
    public void ReadValueFromRequestHeader_WithHeaderName_HeaderNotFoud_WithNotNullDefault_Succeeds()
    {
        // Arrange...
        var anyHeaderValue = It.IsAny<Microsoft.Extensions.Primitives.StringValues>();

        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpRequest = new Mock<HttpRequest>();
        var mockHeaders = new Mock<IHeaderDictionary>();

        mockHeaders.Setup(r => r.TryGetValue(It.IsAny<string>(), out anyHeaderValue)).Returns(false);
        mockHttpRequest.Setup(r => r.Headers).Returns(mockHeaders.Object);
        mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);

        var faker = FakerProvider.GetFaker();
        var fakeDefaultValue = faker.Internet.Random.Word();

        // Act...
        var result = mockHttpContext.Object.ReadValueFromRequestHeader(faker.Internet.Random.Word(), fakeDefaultValue);

        // Assert...
        Assert.NotNull(result);
        Assert.Equal(fakeDefaultValue, result);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void ReadValueFromRequestHeader_WithHeaderName_HeaderValueIsEmptyOrWitheSpaceOnly_ReturnsNull_Succeeds(string headerValue)
    {
        // Arrange...
        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpRequest = new Mock<HttpRequest>();
        var mockHeaders = new Mock<IHeaderDictionary>();

        var testHeaderValue = new Microsoft.Extensions.Primitives.StringValues(headerValue);

        mockHeaders.Setup(r => r.TryGetValue(It.IsAny<string>(), out testHeaderValue)).Returns(true);
        mockHttpRequest.Setup(r => r.Headers).Returns(mockHeaders.Object);
        mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);

        var faker = FakerProvider.GetFaker();

        // Act...
        var result = mockHttpContext.Object.ReadValueFromRequestHeader(faker.Internet.Random.Word(), It.IsAny<string>());

        // Assert...
        Assert.Null(result);
    }
}
