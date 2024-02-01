#if !NET7_0_OR_GREATER

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that this constructor sets all required members for the current type, and callers do not need to set any required members themselves.
/// </summary>
/// <remarks>
/// Reserved to be used by the compiler for tracking metadata. This class should not be used by developers in source code.
/// </remarks>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
internal sealed class SetsRequiredMembersAttribute : Attribute
{
}

#endif