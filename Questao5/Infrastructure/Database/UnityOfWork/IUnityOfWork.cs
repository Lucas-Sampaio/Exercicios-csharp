namespace Questao5.Infrastructure.Database.UnityOfWork
{
    public interface IUnityOfWork
    {
        IUnitOfWorkTransaction? CurrentTransaction { get; }
        ValueTask<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct);
    }
}
