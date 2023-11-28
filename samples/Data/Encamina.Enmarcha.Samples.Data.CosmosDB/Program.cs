using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.Cosmos;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Encamina.Enmarcha.Samples.Data.CosmosDB;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var hostBuilder = new HostBuilder().ConfigureAppConfiguration((configuration) =>
        {
            configuration.AddJsonFile(path: @"appsettings.json", optional: false, reloadOnChange: true);
            configuration.AddEnvironmentVariables();
        });

        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddCosmos(hostContext.Configuration);
            services.AddScoped<IAsyncRepository<Bill>>(sp => sp.GetRequiredService<ICosmosRepositoryFactory>()
                .Create<Bill>(hostContext.Configuration.GetValue<string>("CosmosDBContainerName")));
        });

        var host = hostBuilder.Build();

        while (true)
        {
            Console.WriteLine("Select an option:\n0 - Exit\n1 - Add bill\n2 - Watch bills\n");
            var input = Console.ReadLine();
            var repository = host.Services.GetRequiredService<IAsyncRepository<Bill>>();

            switch (input)
            {
                case "1":
                    Console.WriteLine("Concept:");
                    var concept = Console.ReadLine();
                    Console.WriteLine("Amount:");
                    var amount = double.Parse(Console.ReadLine());
                    var bill = new Bill() { Id = Guid.NewGuid().ToString(), Concept = concept, Amount = amount };
                    await repository.AddAsync(bill, CancellationToken.None);
                    var bills = new Bills(host.Services.GetRequiredService<IAsyncRepository<Bill>>());
                    await bills.AddBillAsync(bill, CancellationToken.None);
                    break;
                case "2":
                    var values = await repository.GetAllAsync(CancellationToken.None);
                    foreach (var value in values)
                    {
                        Console.WriteLine($"{value.Concept}: {value.Amount}");
                    }

                    break;
                case "0":
                    return;
            }

            Console.WriteLine("\n");
        }
    }
}
