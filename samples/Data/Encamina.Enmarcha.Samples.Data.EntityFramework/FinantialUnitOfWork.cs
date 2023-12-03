using Encamina.Enmarcha.Data.EntityFramework;

namespace Encamina.Enmarcha.Samples.Data.EntityFramework;

internal class FinantialUnitOfWork : FullUnitOfWork
{
    internal FinantialUnitOfWork(FinantialDBContext dBContext) : base(dBContext)
    {
    }
}
