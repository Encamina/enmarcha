﻿using System.ComponentModel;

namespace System.Runtime.CompilerServices;

#if !NET5_0_OR_GREATER

/// <summary>
/// Reserved to be used by the compiler for tracking metadata.
/// This class should not be used by developers in source code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit
{
}

#endif // !NET5_0_OR_GREATER