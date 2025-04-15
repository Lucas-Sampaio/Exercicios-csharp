using System.Data;

namespace Questao5.Infrastructure.Database.UnityOfWork
{
    public interface IUnitOfWorkTransaction : IDisposable
    {
        bool Finished { get; }
        IDbTransaction? Transaction { get; }

        void Commit();

        void RollBack();
    }
}