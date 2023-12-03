namespace Encamina.Enmarcha.Samples.Data.EntityFramework;

internal class Bill : IEntity
{
    public string Id { get; set; }

    public string Concept { get; set; }

    public double Amount { get; set; }

    public string EmployeeId { get; set; }
}
