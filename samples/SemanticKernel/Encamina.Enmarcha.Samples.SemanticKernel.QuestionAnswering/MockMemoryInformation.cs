using Encamina.Enmarcha.SemanticKernel.Abstractions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering;

internal class MockMemoryInformation
{
    private readonly IMemoryManager memoryManager;
    private readonly IMemoryStore memoryStore;

    public MockMemoryInformation(IMemoryManager memoryManager, IMemoryStore memoryStore)
    {
        this.memoryManager = memoryManager;
        this.memoryStore = memoryStore;
    }

    public async Task CreateCollection()
    {
        await memoryStore.CreateCollectionAsync(collectionName: "my-collection", CancellationToken.None);
    }

    public async Task SaveDataMockAsync(Kernel kernel)
    {
        var memoryId = Guid.NewGuid().ToString();

        var firstChunkText = @"The Industrial Revolution, also known as the First Industrial Revolution, was a period of global transition of human economy towards more widespread, efficient and stable manufacturing processes that succeeded the Agricultural Revolution, starting from Great Britain, continental Europe, and the United States, that occurred during the period from around 1760 to about 1820–1840.";
        var secondChunkText = @"This transition included going from hand production methods to machines; new chemical manufacturing and iron production processes; the increasing use of water power and steam power; the development of machine tools; and the rise of the mechanized factory system.";
        var thirdChunkText = @"Output greatly increased, and the result was an unprecedented rise in population and the rate of population growth. The textile industry was the first to use modern production methods, and textiles became the dominant industry in terms of employment, value of output, and capital invested.";

        Console.WriteLine("# Inserting the following chunks...");
        Console.WriteLine($"# First chunk: {firstChunkText}  \n");
        Console.WriteLine($"# Second chunk: {secondChunkText}  \n");
        Console.WriteLine($"# Third chunk: {thirdChunkText}   \n");

        await memoryManager.UpsertMemoryAsync(memoryId: memoryId, collectionName: "my-collection", chunks: new List<string> { firstChunkText, secondChunkText, thirdChunkText }, kernel, cancellationToken: CancellationToken.None);
    }
}
