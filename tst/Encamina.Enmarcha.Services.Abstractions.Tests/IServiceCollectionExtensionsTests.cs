using System.Globalization;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Encamina.Enmarcha.Services.Abstractions.Tests;

public sealed class IServiceCollectionExtensionsTests
{
    [Fact]
    public void AddExecutionContext_WithTypes_NullServiceCollection_ThrowsException()
    {
        // Arrange...
        IServiceCollection services = null;

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), typeof(ExecutionContext), typeof(IExecutionContextTemplate), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"collection", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_NullExecutionContextServiceType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(null, typeof(ExecutionContext), typeof(IExecutionContextTemplate), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"executionContextServiceType", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_NullExecutionContextImplementationType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), null, typeof(IExecutionContextTemplate), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"executionContextImplementationType", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_NullExecutionContextTemplateServiceType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), typeof(ExecutionContext), null, typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"executionContextTemplateServiceType", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_NullExecutionContextTemplateImplementationType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), typeof(ExecutionContext), typeof(IExecutionContextTemplate), null));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"executionContextTemplateImplementationType", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_ExecutionContextServiceType_IsNotInterface_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(ExecutionContextTests), typeof(ExecutionContext), typeof(IExecutionContextTemplate), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextServiceType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_ExecutionContextTemplateServiceType_IsNotInterface_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), typeof(ExecutionContext), typeof(ExecutionContextTemplateTests), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextTemplateServiceType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_ExecutionContextServiceType_IsInvalidInterfaceType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IErrorDummy), typeof(ExecutionContext), typeof(IExecutionContextTemplate), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextServiceType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_ExecutionContextTemplateServiceType_IsInvalidInterfaceType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), typeof(ExecutionContext), typeof(IErrorDummy), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextTemplateServiceType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_ExecutionContextImplementationType_DoesNotImplementExpectedInterfaceType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), typeof(It.IsAnyType), typeof(IExecutionContextTemplate), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextImplementationType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_ExecutionContextTemplateImplementationType__DoesNotImplementExpectedInterfaceType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), typeof(ExecutionContext), typeof(IExecutionContextTemplate), typeof(It.IsAnyType)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextTemplateImplementationType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_ExecutionContextTemplateImplementationType_IsNotContructorParameterIn_ExecutionContextImplementationType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(() => services.AddExecutionContext(typeof(IExecutionContext), typeof(InvalidValidExecutionContext), typeof(IExecutionContextTemplate), typeof(ExecutionContextTemplate)));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextImplementationType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithTypes_Succeeds()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        services.AddExecutionContext(typeof(IValidExecutionContext), typeof(ValidExecutionContext), typeof(IValidExecutionContextTemplate), typeof(ValidExecutionContextTemplate));

        // Assert...
        Assert.NotEmpty(services);

        Assert.True(services.Count == 6);

        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IExecutionContext) && sd.ImplementationType == typeof(ValidExecutionContext)));
        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IExecutionContextTemplate) && sd.ImplementationType == typeof(ValidExecutionContextTemplate)));

        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IValidExecutionContext) && sd.ImplementationType == typeof(ValidExecutionContext)));
        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IValidExecutionContextTemplate) && sd.ImplementationType == typeof(ValidExecutionContextTemplate)));

        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(ValidExecutionContext) && sd.ImplementationType == typeof(ValidExecutionContext)));
        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(ValidExecutionContextTemplate) && sd.ImplementationType == typeof(ValidExecutionContextTemplate)));
    }

    [Fact]
    public void AddExecutionContext_CustomImplementations_NullServiceCollection_ThrowsException()
    {
        // Arrange...
        IServiceCollection services = null;

        // Act...
        var exception = Record.Exception(services.AddExecutionContext<ExecutionContext, ExecutionContextTemplate>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"collection", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_CustomImplementations_Succeeds()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        services.AddExecutionContext<ExecutionContext, ExecutionContextTemplate>();

        // Assert...
        Assert.NotEmpty(services);

        Assert.True(services.Count == 4);

        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IExecutionContext) && sd.ImplementationType == typeof(ExecutionContext)));
        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IExecutionContextTemplate) && sd.ImplementationType == typeof(ExecutionContextTemplate)));

        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(ExecutionContext) && sd.ImplementationType == typeof(ExecutionContext)));
        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(ExecutionContextTemplate) && sd.ImplementationType == typeof(ExecutionContextTemplate)));
    }

    [Fact]
    public void AddExecutionContext_WithGenericTypeParameter_NullServiceCollection_ThrowsException()
    {
        // Arrange...
        IServiceCollection services = null;

        // Act...
        var exception = Record.Exception(services.AddExecutionContext<IExecutionContext, ExecutionContext, IExecutionContextTemplate, ExecutionContextTemplate>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"collection", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithGenericTypeParameters_ExecutionContextServiceType_IsNotInterface_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(services.AddExecutionContext<ExecutionContext, ExecutionContext, IExecutionContextTemplate, ExecutionContextTemplate>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextServiceType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithGenericTypeParameter_ExecutionContextTemplateServiceType_IsNotInterface_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(services.AddExecutionContext<IExecutionContext, ExecutionContext, ExecutionContextTemplate, ExecutionContextTemplate>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextTemplateServiceType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithGenericTypeParameter_ExecutionContextTemplateImplementationType_IsNotContructorParameterIn_ExecutionContextImplementationType_ThrowsException()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        var exception = Record.Exception(services.AddExecutionContext<IExecutionContext, InvalidValidExecutionContext, IExecutionContextTemplate, ExecutionContextTemplate>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"executionContextImplementationType", ((ArgumentException)exception).ParamName);
    }

    [Fact]
    public void AddExecutionContext_WithGenericTypeParameters_Succeeds()
    {
        // Arrange...
        var services = new ServiceCollection();

        // Act...
        services.AddExecutionContext<IValidExecutionContext, ValidExecutionContext, IValidExecutionContextTemplate, ValidExecutionContextTemplate>();

        // Assert...
        Assert.NotEmpty(services);

        Assert.True(services.Count == 6);

        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IExecutionContext) && sd.ImplementationType == typeof(ValidExecutionContext)));
        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IExecutionContextTemplate) && sd.ImplementationType == typeof(ValidExecutionContextTemplate)));

        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IValidExecutionContext) && sd.ImplementationType == typeof(ValidExecutionContext)));
        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(IValidExecutionContextTemplate) && sd.ImplementationType == typeof(ValidExecutionContextTemplate)));

        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(ValidExecutionContext) && sd.ImplementationType == typeof(ValidExecutionContext)));
        Assert.Single(services.Where(sd => sd.Lifetime == ServiceLifetime.Scoped && sd.ServiceType == typeof(ValidExecutionContextTemplate) && sd.ImplementationType == typeof(ValidExecutionContextTemplate)));
    }

#pragma warning disable SA1201 // Elements should appear in the correct order

    private interface IErrorDummy
    {
    }

    private interface IValidExecutionContext : IExecutionContext
    {
    }

    private interface IValidExecutionContextTemplate : IExecutionContextTemplate
    {
    }

    private class ValidExecutionContext : ValidExecutionContextTemplate, IValidExecutionContext
    {
        public ValidExecutionContext() : this(null)
        {
        }

        public ValidExecutionContext(IValidExecutionContextTemplate validExecutionContextTemplate)
        {
        }

        public Guid Id => Guid.NewGuid();

        object IIdentifiable.Id => Id;
    }

    private class InvalidValidExecutionContext : ValidExecutionContextTemplate, IValidExecutionContext
    {
        public InvalidValidExecutionContext()
        {
        }

        public Guid Id => Guid.NewGuid();

        object IIdentifiable.Id => Id;
    }

    private class ValidExecutionContextTemplate : IValidExecutionContextTemplate
    {
        public string CorrelationId { get; init; }

        public string CorrelationCallId { get; init; }

        public CultureInfo CultureInfo { get; init; }

        public CancellationToken CancellationToken { get; init; }

        public IConfiguration Configuration { get; init; }
    }

#pragma warning restore SA1201 // Elements should appear in the correct order
}
