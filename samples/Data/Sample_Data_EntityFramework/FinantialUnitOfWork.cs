using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework;

namespace Sample_Data_EntityFramework;

public class FinantialUnitOfWork : FullUnitOfWork, IFullUnitOfWork
{
    public FinantialUnitOfWork(FinantialDBContext dBContext)
        : base(dBContext)
    {
    }
}
