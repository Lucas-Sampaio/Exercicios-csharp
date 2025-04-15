using System.Data;

namespace Questao5.Infrastructure.Database.UnityOfWork
{
    public class DapperUnityOfWorkTransaction : IUnitOfWorkTransaction
    {
        public IDbTransaction Transaction { get; }

        public DapperUnityOfWorkTransaction(IDbTransaction dbTransaction)
        {
            Transaction = dbTransaction;
        }

        public bool Finished { get; private set; }

        public void Commit()
        {
            EnsureTransactionHasntFinished();
            try
            {
                Transaction.Commit();
            }
            finally
            {
                Finished = true;
            }
        }

        public void Dispose()
        {
            try
            {
                Transaction?.Dispose();
            }
            finally
            {
                Finished = true;
            }
        }

        public void RollBack()
        {
            EnsureTransactionHasntFinished();
            try
            {
                Transaction.Rollback();
            }
            finally
            {
                Finished = true;
            }
        }

        private void EnsureTransactionHasntFinished()
        {
            if (Finished)
                throw new Exception("Transacao ja finalizada");
        }
    }
}