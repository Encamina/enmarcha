using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework;

namespace Sample_Data_EntityFramework;
public class MyFullUnitOfWork : FullUnitOfWork, IFullUnitOfWork
{
    public MyFullUnitOfWork(MyDBContext dBContext)
        : base(dBContext)
    {
    }
}
