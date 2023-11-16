using Encamina.Enmarcha.Bot.Abstractions.Greetings;

using Microsoft.Bot.Builder;

namespace Encamina.Enmarcha.Bot.Greetings;

/// <summary>
/// Base or common implementation of a <see cref="IGreetingsProvider">greetings provider</see>.
/// </summary>
internal abstract class GreetingsProviderBase : IGreetingsProvider
{
    /// <summary>
    /// Gets the current properties for a template that represents a greetings message.
    /// </summary>
    protected internal IDictionary<string, object> GreetingsTemplateProperties { get; private set; }

    /// <inheritdoc/>
    public void AddGreetingsTemplateProperties(IDictionary<string, object> templateProperties) => GreetingsTemplateProperties = templateProperties;

    /// <inheritdoc/>
    public abstract Task SendAsync(ITurnContext turnContext, CancellationToken cancellationToken);
}
