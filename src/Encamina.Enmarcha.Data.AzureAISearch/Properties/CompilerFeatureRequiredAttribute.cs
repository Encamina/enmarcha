#if !NET7_0_OR_GREATER

namespace System.Runtime.CompilerServices;

/// <summary>
/// Indicates that compiler support for a particular feature is required for the location where this attribute is applied.
/// </summary>
/// <remarks>
/// Reserved to be used by the compiler for tracking metadata. This class should not be used by developers in source code.
/// </remarks>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute
{
    /// <summary>
    /// The <see cref="FeatureName"/> used for the ref structs C# feature.
    /// </summary>
    public const string RefStructs = nameof(RefStructs);

    /// <summary>
    /// The <see cref="FeatureName"/> used for the required members C# feature.
    /// </summary>
    public const string RequiredMembers = nameof(RequiredMembers);

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilerFeatureRequiredAttribute"/> class.
    /// </summary>
    /// <param name="featureName">The name of the required compiler feature.</param>
    public CompilerFeatureRequiredAttribute(string featureName)
    {
        FeatureName = featureName;
    }

    /// <summary>
    /// Gets the name of the compiler feature.
    /// </summary>
    public string FeatureName { get; }

    /// <summary>
    /// Gets a value indicating whether the compiler can choose to allow access to the location
    /// where this attribute is applied if it does not understand <see cref="FeatureName"/>.
    /// </summary>
    public bool IsOptional { get; init; }
}

#endif