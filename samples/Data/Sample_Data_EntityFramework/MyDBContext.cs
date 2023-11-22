using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Sample_Data_EntityFramework;

public interface IEntity { }

public class Employee : IEntity
{
    public string Id { get; set; }

    public string FullName { get; set; }
}

public class Bill : IEntity
{
    public string Id { get; set; }

    public string Concept { get; set; }

    public double Amount { get; set; }

    public string EmployeeId { get; set; }
}

public class MyDBContext : DbContext
{
    public DbSet<Bill> Bills { get; set; }

    public DbSet<Employee> Employees { get; set; }
}