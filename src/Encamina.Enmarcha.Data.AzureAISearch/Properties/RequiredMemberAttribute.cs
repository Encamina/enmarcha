#if !NET7_0_OR_GREATER

namespace System.Runtime.CompilerServices;

/// <summary>
/// Specifies that a type has required members or that a member is required.
/// </summary>
/// <remarks>
/// Reserved to be used by the compiler for tracking metadata. This class should not be used by developers in source code.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute
{
}

#endif