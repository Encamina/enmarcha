using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// Base representation of an intent kind (type).
/// </summary>
[SuppressMessage(@"Major Code Smell",
                 "S4035:Classes implementing \"IEquatable<T>\" should be sealed",
                 Justification = "Intended useage of this class requires it to be inheritable and implement the \"IEquatable<T>\" interface!")]
public class IntentKindBase : IEqualityComparer<IntentKindBase>, IEquatable<IntentKindBase>
{
    private readonly string value;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntentKindBase"/> class.
    /// </summary>
    /// <param name="value">The value of this kind (type) of intent.</param>
    protected IntentKindBase(string value)
    {
        this.value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary> Converts a string to a <see cref="IntentKindBase"/>.</summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator IntentKindBase(string value) => new(value);

    /// <summary>
    /// Determines if two <see cref="IntentKindBase"/> values are the same.
    /// </summary>
    /// <param name="left">The left part from the equality comparisson.</param>
    /// <param name="right">The right part from the equality comparisson.</param>
    /// <returns>
    /// Returns <see langword="true"/> if <paramref name="left"/> is the same as <paramref name="right"/>, otherwise returns <see langword="false"/>.
    /// </returns>
    public static bool operator ==(IntentKindBase left, IntentKindBase right) => left.Equals(right);

    /// <summary>
    /// Determines if two <see cref="IntentKindBase"/> values are <b>not</b> the same.
    /// </summary>
    /// <param name="left">The left part from the inequality comparison.</param>
    /// <param name="right">The right part from the inequality comparison.</param>
    /// <returns>
    /// Returns <see langword="true"/> if <paramref name="left"/> is <b>not</b> the same as <paramref name="right"/>, otherwise returns <see langword="false"/>.
    /// </returns>
    public static bool operator !=(IntentKindBase left, IntentKindBase right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(IntentKindBase other) => string.Equals(value, other.value, StringComparison.InvariantCultureIgnoreCase);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is IntentKindBase other && Equals(other);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => value?.GetHashCode() ?? 0;

    /// <inheritdoc />
    public override string ToString() => value;

    /// <inheritdoc />
    public bool Equals(IntentKindBase x, IntentKindBase y) => (x == null && y == null) || (x != null && y != null && x.Equals(y));

    /// <inheritdoc />
    public int GetHashCode(IntentKindBase obj) => obj != null ? obj.GetHashCode() : 0;
}
