using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Represents an OpenAI competion service which from some input text as a prompt, will generate a text completion that attempts
/// to match whatever context or pattern has been given to an underlaying model.
/// </summary>
/// <remarks>
/// For example, for a prompt like <i>“Write a tagline for an ice cream shop”</i>, a model may return a completion like <i>“We serve up smiles with every scoop!”</i>.
/// </remarks>
public interface ICompletionService : IIdentifiable<string>, INameable
{
    /// <summary>
    /// Creates a completion for the provided prompt and parameters.
    /// </summary>
    /// <param name="request">A request with prompts and parameters for the completion.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The result with the completion from the request.</returns>
    Task<CompletionResult> CompleteAsync(CompletionRequest request, CancellationToken cancellationToken);
}
