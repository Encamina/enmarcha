using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.DependencyInjection;

/// <summary>
/// Enables a type to automatically register itself into the dependency injection container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class AutoRegisterServiceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoRegisterServiceAttribute"/> class.
    /// </summary>
    /// <param name="serviceLifetime">The service lifetime to use when registering the class.</param>
    /// <param name="alternativeTypes">Alternative types that should be mapped to this type when registering it.</param>
    public AutoRegisterServiceAttribute(ServiceLifetime serviceLifetime, params Type[] alternativeTypes)
    {
        AlternativeTypes = alternativeTypes;
        ServiceLifetime = serviceLifetime;
    }

    /// <summary>
    /// Gets the service lifetime to use when registering the class.
    /// </summary>
    public ServiceLifetime ServiceLifetime { get; }

    /// <summary>
    /// Gets the alternative types that should be mapped to this type when registering it.
    /// </summary>
    public Type[] AlternativeTypes { get; }

    /// <summary>
    /// <para>
    /// Gets or sets a value indicating whether, if the class implements one or more interfaces, it should be registered only as implementation type. Default is <see langword="false"/>.
    /// </para>
    /// <para>
    /// Set as <see langword="true"/> to force registering the class only as an implementation type, excluding from the registration any implemented interfaces.
    /// </para>
    /// </summary>
    /// <remarks>
    /// There are some scenarios when a class should be registered only as an implementation type, regardless the interfaces it implements.
    /// </remarks>
    public bool ForceOnlyAsImplementationType { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether interfaces directly implemented by the class should be registered as service types. Default is <see langword="true"/>.
    /// </summary>
    public bool RegisterInterfaces { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether interfaces inherited by the class (i.e., implemetend by base classes) should be registered as services types. Default is <see langword="false"/>.
    /// </summary>
    public bool IncludeInheritedInterfaces { get; set; } = false;
}
