using Encamina.Enmarcha.Data.Abstractions;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sample_Data_EntityFramework;

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        var hostBuilder = new HostBuilder();

        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddDbContext<FinantialDBContext>(opt =>
            {
                opt.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IFullUnitOfWork, FinantialUnitOfWork>();
        });

        var host = hostBuilder.Build();

        while (true)
        {
            Console.Write("Select an option: \n0 - exit\n1 - Add bill\n2 - Add employee\n3 - Get all bills");
            var input = Console.ReadLine();
            var finantialSystemOperations = new FinantialOperations(host.Services.GetRequiredService<IFullUnitOfWork>());

            switch (input)
            {
                case "0":
                    return;
                case "1":
                    Console.WriteLine("Select the employee who created the bill:");
                    var employees = await finantialSystemOperations.GetAllAsync<Employee>(CancellationToken.None);
                    var employeeDictionary = new Dictionary<int, Employee>();

                    for (var i = 0; i < employees.Count(); i++)
                    {
                        var employee = (Employee)employees[i];
                        employeeDictionary.Add(i + 1, employee);
                        Console.WriteLine($"{i + 1} - {employee.FullName}");
                    }

                    var selectedNumber = int.Parse(Console.ReadLine());

                    if (employeeDictionary.TryGetValue(selectedNumber, out var selectedEmployee))
                    {
                        Console.WriteLine("Concept of the bill:");
                        var concept = Console.ReadLine();
                        Console.WriteLine("Amount of the bill:");
                        var amount = double.Parse(Console.ReadLine());
                        await finantialSystemOperations.AddAsync(new Bill() { Amount = amount, Concept = concept, EmployeeId = selectedEmployee.Id }, CancellationToken.None);
                        Console.WriteLine("Bill succesfully uploaded");
                    }

                    break;
                case "2":
                    Console.WriteLine("Full name of the employee");
                    break;
            }
        }
    }
}