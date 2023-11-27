using Encamina.Enmarcha.Data.Abstractions;

namespace Sample_Data_CosmosDB;

public class Bills
{
    private readonly IAsyncRepository<Bill> billsRepository;

    public Bills(IAsyncRepository<Bill> billsRepository)
    {
        this.billsRepository = billsRepository;
    }

    public async Task AddBillAsync(Bill bill, CancellationToken cancellationToken)
    {
        await billsRepository.AddAsync(bill, cancellationToken);
        Console.WriteLine("Bill added");
    }
}
