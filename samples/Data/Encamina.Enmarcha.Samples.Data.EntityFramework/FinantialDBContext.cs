using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Samples.Data.EntityFramework;

internal class FinantialDBContext : DbContext
{
    public DbSet<Bill> Bills { get; set; }

    public DbSet<Employee> Employees { get; set; }
}