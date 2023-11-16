using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Request for completions request for OpenAI.
/// </summary>
public class CompletionRequest
{
    private readonly int? bestOf = 1;
    private readonly float frequencyPenalty = 0;
    private readonly int maxTokens = 16;
    private readonly int numberOfCompletionsPerPrompt = 1;
    private readonly float presencePenalty = 0;
    private readonly float temperature = 1;
    private readonly float topProbability = 1;

    private readonly IEnumerable<string> prompts = Enumerable.Empty<string>();
    private readonly IEnumerable<string> stops;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompletionRequest"/> class.
    /// </summary>
    public CompletionRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompletionRequest"/> class from another <see cref="CompletionRequest"/>
    /// as template.
    /// </summary>
    /// <param name="template">A <see cref="CompletionRequest"/> to use as template for this instance.</param>
    public CompletionRequest(CompletionRequest template)
    {
        BestOf = template.BestOf;
        DoEcho = template.DoEcho;
        FrequencyPenalty = template.FrequencyPenalty;
        MaxTokens = template.MaxTokens;
        NumberOfCompletionsPerPrompt = template.NumberOfCompletionsPerPrompt;
        PresencePenalty = template.PresencePenalty;
        Prompts = template.Prompts;
        StopSequences = template.StopSequences;
        Temperature = template.Temperature;
        TopProbability = template.TopProbability;
        UserId = template.UserId;
    }

    /// <summary>
    /// Gets the best completions server-side, and returns them (the one with the lowest log probability per token). When used
    /// with '<see cref="NumberOfCompletionsPerPrompt"/>' (<c>n</c>), this  property controls the number of candidate completions
    /// and '<see cref="NumberOfCompletionsPerPrompt"/>' (<c>n</c>) specifies how many to return. Corresponds to '<c>best_of</c>'.
    /// Defaults to <c>1</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b>Important</b> The value of this property must be greater than '<see cref="NumberOfCompletionsPerPrompt"/>' (<c>n</c>).
    /// </para>
    /// <para>
    ///     <b>Note 1</b>: Because this parameter generates many completions, it can quickly consume the token quota. Use carefully and ensure
    ///     the settings for '<see cref="MaxTokens"/>' (<c>max_tokens</c>) and '<see cref="StopSequences"/>' (<c>stop</c>) are reasonable and correct.
    /// </para>
    /// <para>
    ///     <b>Note 2</b>: Results can't be streamed.
    /// </para>
    /// </remarks>
    public int? BestOf
    {
        get => bestOf;
        init
        {
            if (value == null)
            {
                bestOf = null;
                return;
            }

            Guard.IsGreaterThanOrEqualTo(value.Value, 1);
            bestOf = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether to echo back the prompt in addition to the completion. Corresponds to '<c>echo</c>'.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool? DoEcho { get; init; } = false;

    /// <summary>
    /// Gets a number between <c>-2.0</c> and <c>2.0</c>. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing
    /// the model's likelihood to repeat the same line verbatim. Corresponds to '<c>frequency_penalty</c>'. Defaults to <c>0</c>.
    /// </summary>
    public float FrequencyPenalty
    {
        get => frequencyPenalty;
        init
        {
            Guard.IsGreaterThanOrEqualTo(value, -2.0);
            Guard.IsLessThanOrEqualTo(value, 2.0);
            frequencyPenalty = value;
        }
    }

    /// <summary>
    /// Gets the maximum number of tokens to generate in the completion. Corresponds to '<c>max_tokens</c>'. Defaults to <c>16</c>.
    /// </summary>
    /// <remarks>
    /// The token count of '<see cref="Prompts"/>' plus '<see cref="MaxTokens"/>' (<c>max_tokens</c>) can't exceed the model's context length.
    /// Most models have a context length of 2048 tokens (except <c>davinci-codex</c>, which supports 4096).
    /// </remarks>
    public int MaxTokens
    {
        get => maxTokens;
        init
        {
            Guard.IsGreaterThanOrEqualTo(value, 1);
            maxTokens = value;
        }
    }

    /// <summary>
    /// Gets how many completions to generate for each prompt. Corresponds to '<c>n</c>'. Defaults to <c>1</c>.
    /// </summary>
    /// <remarks>
    /// <b>Important</b>: Because this parameter generates many completions, it can quickly consume the token quota. Use carefully and ensure the settings
    /// for '<see cref="MaxTokens"/>' (<c>max_tokens</c>) and '<see cref="StopSequences"/>' (<c>stop</c>) are reasonable and correct.
    /// </remarks>
    public int NumberOfCompletionsPerPrompt
    {
        get => numberOfCompletionsPerPrompt;
        init
        {
            Guard.IsGreaterThanOrEqualTo(value, 1);
            numberOfCompletionsPerPrompt = value;
        }
    }

    /// <summary>
    /// Gets a number between <c>-2.0</c> and <c>2.0</c>. Positive values penalize new tokens based on whether they appear in the text so far, increasing
    /// the model's likelihood to talk about new topics. Corresponds to '<c>presence_penalty</c>'. Defaults to <c>0</c>.
    /// </summary>
    public float PresencePenalty
    {
        get => presencePenalty;
        init
        {
            Guard.IsGreaterThanOrEqualTo(value, -2.0);
            Guard.IsLessThanOrEqualTo(value, 2.0);
            presencePenalty = value;
        }
    }

    /// <summary>
    /// Gets the prompt(s) to generate completions for, encoded as a string. Corresponds to '<c>prompt</c>'.
    /// </summary>
    public IEnumerable<string> Prompts
    {
        get => prompts;
        init
        {
            Guard.IsNotNull(value);
            prompts = value;
        }
    }

    /// <summary>
    /// Gets up to four (4) sequences to detect and stop generating (completing) further tokens. The response won't contain the stop sequence.
    /// Corresponds to '<c>stop</c>'. Defaults to <see langword="null"/>, which usually means that the stop sequence will '<c>\r\n</c>'.
    /// </summary>
    [SuppressMessage(@"Minor Code Smell",
                     @"S3236:Caller information arguments should not be provided explicitly",
                     Justification = @"Currently the `Guard` library doesn't provide IEnumerable<string> support forcing the validation to create an Array which modifies the expected name of the invalid parameter.")]
    public IEnumerable<string> StopSequences
    {
        get => stops;
        init
        {
            if (value != null)
            {
                Guard.HasSizeLessThanOrEqualTo(value.ToArray(), 4, nameof(value));

                stops = value;
            }
        }
    }

    /// <summary>
    /// Gets the sampling temperature to use. Higher values means the model will take more risks. Corresponds to '<c>temperature</c>'.
    /// Defaults to <c>1</c>.
    /// </summary>
    /// <remarks>
    /// For example, a value of <c>0.9</c> might be more suitable for more creative applications, and <c>0</c> (a.k.a. '<c>argmax</c> sampling') for applications that require
    /// well-defined answers. It is generally recommend to alter either this property or '<see cref="TopProbability"/>' (<c>top_p</c>), but not both.
    /// </remarks>
    public float Temperature
    {
        get => temperature;
        init
        {
            Guard.IsGreaterThanOrEqualTo(value, 0);
            Guard.IsLessThanOrEqualTo(value, 1);
            temperature = value;
        }
    }

    /// <summary>
    /// Gets the top probability (sometimes called nucleus sampling mass), which represents an alternative to sampling with '<see cref="Temperature"/>', where
    /// the model considers the results of the tokens with top probability mass. Corresponds to '<c>top_p</c>'. Defaults to <c>1</c>.
    /// </summary>
    /// <remarks>
    /// For example, <c>0.1</c> means only the tokens comprising the top 10% probability mass are considered. It is generally recommend to alter either this property
    /// or '<see cref="Temperature"/>', but not both.
    /// </remarks>
    public float TopProbability
    {
        get => topProbability;
        init
        {
            Guard.IsGreaterThanOrEqualTo(value, 0);
            Guard.IsLessThanOrEqualTo(value, 1);
            topProbability = value;
        }
    }

    /// <summary>
    /// Gets a unique identifier representing the end-user requesting the completion, which can help monitoring and detecting abuse.
    /// </summary>
    public string UserId { get; init; } = null;
}
