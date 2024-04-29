using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Abstractions.QuestionAnswering;

/// <summary>
/// A result from sending a response.
/// </summary>
public class SendResponseResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SendResponseResult"/> class.
    /// </summary>
    public SendResponseResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SendResponseResult"/> class.
    /// </summary>
    /// <param name="resourceResponse">The <see cref="ResourceResponse"/> associated to this result.</param>
    public SendResponseResult(ResourceResponse resourceResponse)
    {
        ResourceResponse = resourceResponse;
    }

    /// <summary>
    /// Gets an empty send response result.
    /// </summary>
    public static SendResponseResult Empty => new() { ResourceResponse = null };

    /// <summary>
    /// Gets a value indicating whether this result of sending a response was successful or not.
    /// </summary>
    public bool Successful => !string.IsNullOrWhiteSpace(ResourceResponse?.Id);

    /// <summary>
    /// Gets the resource response.
    /// </summary>
    public ResourceResponse? ResourceResponse { get; init; }
}
