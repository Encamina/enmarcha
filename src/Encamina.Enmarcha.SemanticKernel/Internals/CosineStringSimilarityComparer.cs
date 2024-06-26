﻿using System.Numerics.Tensors;

using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

namespace Encamina.Enmarcha.SemanticKernel.Internals;

/// <summary>
/// An implementation of the <see cref="IStringSimilarityComparer"/> interface that uses cosine similarity algorithm, using semantic kernel to generate the embeddings.
/// </summary>
internal sealed class CosineStringSimilarityComparer : IStringSimilarityComparer
{
    private readonly Kernel kernel;
    private readonly ITextEmbeddingGenerationService textEmbeddingGenerationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosineStringSimilarityComparer"/> class.
    /// </summary>
    /// <param name="kernel">A valid instance of a <see cref="Kernel"/>.</param>
    public CosineStringSimilarityComparer(Kernel kernel)
    {
        this.kernel = kernel;

        textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
    }

    /// <inheritdoc />
    public async Task<double> CompareAsync(string first, string second, CancellationToken cancellationToken)
    {
        var embeddingsFirst = await textEmbeddingGenerationService.GenerateEmbeddingAsync(first, kernel, cancellationToken);
        var embeddingsSecond = await textEmbeddingGenerationService.GenerateEmbeddingAsync(second, kernel, cancellationToken);

        return CalculateCosineSimilarity(embeddingsFirst, embeddingsSecond);
    }

    private static double CalculateCosineSimilarity(ReadOnlyMemory<float> vec1, ReadOnlyMemory<float> vec2)
    {
        var v1 = vec1.ToArray();
        var v2 = vec2.ToArray();

        if (vec1.Length != vec2.Length)
        {
            throw new ArgumentException($"Vector size should be the same: {vec1.Length} != {vec2.Length}");
        }

        return TensorPrimitives.CosineSimilarity(v1, v2);
    }
}

