using System.ComponentModel.DataAnnotations;

using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.Skills.QuestionAnswering;

/// <summary>
/// Configuration options for the question answering dialog.
/// </summary>
public class QuestionAnsweringSkillOptions
{
    /// <summary>
    /// Gets the name of the Question Answering service to use by this skill.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string QuestionAnsweringServiceName { get; init; }

    /// <summary>
    /// Gets the dialog's name.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string DialogName { get; init; }

    /// <summary>
    /// Gets the dialog's intent.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string DialogIntent { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="QuestionAnsweringDialog"> question answering dialog</see> must
    /// normalize the message (question) removing specific characters (usually diacritics). Defaults to <see langword="false"/>.
    /// </summary>
    public bool NormalizeMessage { get; set; } = false;

    /// <summary>
    /// Gets a collection of characters to remove from the message when normalizing.
    /// </summary>
    public required IList<char> NormalizeRemoveCharacters { get; init; } = new List<char>();

    /// <summary>
    /// Gets a collection of characters to replace from the message when normalizing.
    /// </summary>
    /// <remarks>
    /// This should be a <see cref="IDictionary{TKey, TValue}"/> of <see cref="char"/> as key and value, but
    /// sadly the configuration binder does not correctly convert key-value pairs in a JSON settings file
    /// as characters. So, this property uses strings but internally only considers the first character, any
    /// other is going to be ignored.
    /// </remarks>
    public required IDictionary<string, string> NormalizeReplaceCharacters { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="QuestionAnsweringDialog"> question answering dialog</see>
    /// must answer with the first found answer as a fallback. Default is <see langword="true"/>.
    /// </summary>
    public bool FallbackToFirstAnswer { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="QuestionAnsweringDialog"> question answering dialog</see>
    /// should generate traces (usually `<see cref="ActivityTypes.Trace"/>` activities) or include additional verbose information.
    /// </summary>
    public bool Verbose { get; set; } = false;
}
