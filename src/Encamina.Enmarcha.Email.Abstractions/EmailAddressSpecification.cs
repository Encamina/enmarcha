namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Presents an e-mail address specification.
/// </summary>
public class EmailAddressSpecification
{
    /// <summary>
    /// Gets the address for this e-mail address specififcation.
    /// </summary>
    public string Address { get; init; }

    /// <summary>
    /// Gets the name (usually) associated to the <see cref="Address"/> of this e-mail specification.
    /// </summary>
    public string Name { get; init; }

    /// <inheritdoc />
    public override string ToString() => $@"{Address} <{Name}>";
}
