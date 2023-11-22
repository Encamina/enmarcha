using Encamina.Enmarcha.Data.Abstractions;

namespace Sample_Data_EntityFramework;
public class MyClass
{
    private readonly IFullUnitOfWork uow;

    public MyClass(IFullUnitOfWork uow)
    {
        this.uow = uow;
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken)
        where T : class, IEntity
    {
        var entityRepository = uow.GetAsyncRepository<T>();
        await entityRepository.AddAsync(entity, cancellationToken);
        await uow.SaveAsync(cancellationToken);
    }

    public async Task<List<IEntity>> GetAllAsync<T>(CancellationToken cancellationToken)
        where T : class, IEntity
    {
        var list = new List<IEntity>();
        var entityRepository = uow.GetAsyncRepository<T>();
        list.AddRange(await entityRepository.GetAllAsync(cancellationToken));
        return list;
    }
}