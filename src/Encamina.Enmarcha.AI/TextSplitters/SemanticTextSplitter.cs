using System.Numerics.Tensors;
using System.Text;
using System.Text.RegularExpressions;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.Core;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.TextSplitters;

/// <summary>
/// Implementation of the <see cref="ISemanticTextSplitter"/> interface that utilizes semantic analysis to split a given text into meaningful chunks.
/// It employs a combination of sentence embeddings and cosine similarity to identify breakpoints and create cohesive sentence groups.
/// </summary>
public class SemanticTextSplitter : ISemanticTextSplitter
{
    private static readonly Regex SentenceSplitRegex = new(@"(?<=[.?!])\s+", RegexOptions.Compiled, TimeSpan.FromSeconds(30));

    private SemanticTextSplitterOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticTextSplitter"/> class.
    /// </summary>
    /// <param name="options">The options to use when configuring the semantic text splitter.</param>
    public SemanticTextSplitter(IOptionsMonitor<SemanticTextSplitterOptions> options)
    {
        this.options = options.CurrentValue;

        options.OnChange(newOptions => this.options = newOptions);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> SplitAsync(string text, Func<IList<string>, CancellationToken, Task<IList<ReadOnlyMemory<float>>>> embeddingsGenerator, CancellationToken cancellationToken = default)
    {
        // Code inspired by
        // https://github.com/run-llama/llama_index/blob/8ed753df970f068f6afc8a83fd51a1f40880de9e/llama-index-packs/llama-index-packs-node-parser-semantic-chunking/llama_index/packs/node_parser_semantic_chunking/base.py
        // https://github.com/langchain-ai/langchain/blob/ced5e7bae790cd9ec4e5374f5d070d9f23d6457b/libs/experimental/langchain_experimental/text_splitter.py

        // Splitting the text on '.', '?', and '!'
        var sentences = SentenceSplitRegex.Split(text).Where(t => !string.IsNullOrEmpty(t)).ToList();
        if (sentences.Count == 1)
        {
            return sentences;
        }

        // Combine sentences based on buffer size
        var combinedSentences = CreateCombinedSentences(sentences, options.BufferSize);

        // Generate embeddings for combined sentences
        var combinedSentencesEmbeddings = await embeddingsGenerator(combinedSentences, cancellationToken);

        // Calculate cosine distances between consecutive sentence embeddings
        var distancesToNextSentence = CalculateDistancesToNextSentence(combinedSentencesEmbeddings);

        // Calculate threshold for identifying breakpoints
        var breakpointDistanceThreshold = CalculateBreakpointThreshold(distancesToNextSentence, options.BreakpointThresholdType, options.BreakpointThresholdAmount);

        // Identify indexes above the threshold as breakpoints
        var indexesAboveThreshold = distancesToNextSentence
            .Select((distance, index) => new { Index = index, Distance = distance })
            .Where(item => item.Distance > breakpointDistanceThreshold)
            .Select(item => item.Index)
            .ToList();

        // Slice sentences based on identified breakpoints
        var chunks = SliceSentences(sentences, indexesAboveThreshold);

        return chunks;
    }

    /// <summary>
    /// Combines sentences based on a specified buffer size, creating cohesive groups for further analysis.
    /// Each combined sentence is formed by including neighboring sentences within the specified buffer size before and after the current sentence.
    /// </summary>
    /// <param name="sentences">The list of sentences to be combined.</param>
    /// <param name="bufferSize">The number of sentences to include on each side of the current sentence within the buffer size.</param>
    /// <returns>A list of combined sentences.</returns>
    private static List<string> CreateCombinedSentences(IList<string> sentences, int bufferSize)
    {
        var combinedSentences = new List<string>(sentences.Count);

        // Iterate through each sentence in the input list to create combined sentences
        for (var i = 0; i < sentences.Count; i++)
        {
            var combinedSentenceBuilder = new StringBuilder();

            // Add sentences before the current one, based on the buffer size.
            for (var j = i - bufferSize; j < i; j++)
            {
                if (j >= 0)
                {
                    combinedSentenceBuilder.Append(sentences[j]).Append(' ');
                }
            }

            // Add the current sentence
            combinedSentenceBuilder.Append(sentences[i]);

            // Add sentences after the current one, based on the buffer size
            for (var j = i + 1; j < i + 1 + bufferSize; j++)
            {
                if (j < sentences.Count)
                {
                    combinedSentenceBuilder.Append(' ').Append(sentences[j]);
                }
            }

            combinedSentences.Add(combinedSentenceBuilder.ToString());
        }

        return combinedSentences;
    }

    /// <summary>
    /// Calculates the cosine distances between consecutive sentence embeddings.
    /// </summary>
    /// <param name="embeddings">The list of sentence embeddings to calculate distances.</param>
    /// <returns>A list of cosine distances between consecutive sentence embeddings.</returns>
    private static List<double> CalculateDistancesToNextSentence(IList<ReadOnlyMemory<float>> embeddings)
    {
        var distances = new List<double>(embeddings.Count - 1);

        for (var i = 0; i < embeddings.Count - 1; i++)
        {
            var embeddingCurrent = embeddings[i];
            var embeddingNext = embeddings[i + 1];

            // Calculate cosine similarity
            var similarity = TensorPrimitives.CosineSimilarity(embeddingCurrent.Span, embeddingNext.Span);

            // Convert to cosine distance
            var distance = 1 - similarity;

            distances.Add(distance);
        }

        return distances;
    }

    /// <summary>
    /// Calculates the threshold for identifying breakpoints based on the specified percentile of sorted cosine distances.
    /// </summary>
    /// <param name="distances">The list of cosine distances between sentence embeddings.</param>
    /// <param name="breakpointThresholdType">The type of threshold calculation to be applied.</param>
    /// <param name="breakpointThresholdAmount">The amount used in the threshold calculation.</param>
    /// <returns>The calculated threshold for identifying breakpoints.</returns>
    private static double CalculateBreakpointThreshold(IList<double> distances, BreakpointThresholdType breakpointThresholdType, float breakpointThresholdAmount)
    {
        switch (breakpointThresholdType)
        {
            case BreakpointThresholdType.Percentile:
                return MathUtils.Percentile(distances, breakpointThresholdAmount);
            case BreakpointThresholdType.StandardDeviation:
                return (MathUtils.StandardDeviation(distances) * breakpointThresholdAmount) + distances.Average();
            case BreakpointThresholdType.Interquartile:
                var iqr = MathUtils.InterquartileRange(distances);
                return distances.Average() + (breakpointThresholdAmount * iqr);
            default:
                throw new ArgumentOutOfRangeException(nameof(breakpointThresholdType), breakpointThresholdType, null);
        }
    }

    /// <summary>
    /// Slices the sentences based on the provided indexes, creating chunks of text between breakpoints.
    /// </summary>
    /// <param name="sentences">The list of sentences to be sliced.</param>
    /// <param name="indexes">The list of indexes indicating breakpoints in the sentences.</param>
    /// <returns>A list of sliced text chunks.</returns>
    private static IEnumerable<string> SliceSentences(IList<string> sentences, List<int> indexes)
    {
        var chunks = new List<string>();
        var startIndex = 0;

        // Iterate through the breakpoints to slice the sentences
        foreach (var index in indexes)
        {
            // Slice the sentences from the current start index to the end index
            var group = sentences.Skip(startIndex).Take(index - startIndex + 1).ToList();

            chunks.Add(string.Join(" ", group));

            // Update the start index for the next group
            startIndex = index + 1;
        }

        // The last group, if any sentences remain
        if (startIndex < sentences.Count)
        {
            // Get the remaining sentences after the last breakpoint
            var remainingGroup = sentences.Skip(startIndex).ToList();

            chunks.Add(string.Join(" ", remainingGroup));
        }

        return chunks;
    }
}