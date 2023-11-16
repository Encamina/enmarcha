using System.ComponentModel.DataAnnotations;

using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Skills.QuestionAnswering;

/// <summary>
/// Configuration options for the question answering dialog.
/// </summary>
public class QuestionAnsweringSkillOptions
{
    /// <summary>
    /// Gets or sets the name of the Question Answering service to use by this skill.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string QuestionAnsweringServiceName { get; set; }

    /// <summary>
    /// Gets or sets the dialog's name.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string DialogName { get; set; }

    /// <summary>
    /// Gets or sets the dialog's intent.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string DialogIntent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="QuestionAnsweringDialog"> question answering dialog</see> must
    /// normalize the message (question) removing specific characters (usually diacritics). Defaults to <see langword="false"/>.
    /// </summary>
    public bool NormalizeMessage { get; set; } = false;

    /// <summary>
    /// Gets or sets a collection of characters to remove from the message when normalizing.
    /// </summary>
    public IList<char> NormalizeRemoveCharacters { get; set; } = new List<char>();

    /// <summary>
    /// Gets or sets a collection of characters to replace from the message when normalizing.
    /// </summary>
    /// <remarks>
    /// This should be a <see cref="IDictionary{TKey, TValue}"/> of <see cref="char"/> as key and value, but
    /// sadly the configuration binder does not correctly converts key-value pairs in a JSON settings file
    /// as characters. So, this property uses strings but internally only considers the first character, any
    /// other is going to be ignored.
    /// </remarks>
    public IDictionary<string, string> NormalizeReplaceCharacters { get; set; } = new Dictionary<string, string>();

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
