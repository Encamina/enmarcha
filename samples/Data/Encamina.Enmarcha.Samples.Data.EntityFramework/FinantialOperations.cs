using Encamina.Enmarcha.Data.Abstractions;

namespace Encamina.Enmarcha.Samples.Data.EntityFramework;

internal class FinantialOperations
{
    private readonly IFullUnitOfWork unitOfWork;

    public FinantialOperations(IFullUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken)
        where T : class, IEntity
    {
        var entityRepository = unitOfWork.GetAsyncRepository<T>();
        await entityRepository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
    }

    public async Task<List<IEntity>> GetAllAsync<T>(CancellationToken cancellationToken)
        where T : class, IEntity
    {
        var list = new List<IEntity>();
        var entityRepository = unitOfWork.GetAsyncRepository<T>();
        list.AddRange(await entityRepository.GetAllAsync(cancellationToken));
        return list;
    }
}