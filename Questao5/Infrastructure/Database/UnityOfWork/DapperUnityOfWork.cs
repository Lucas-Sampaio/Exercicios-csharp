using System.Data;

namespace Questao5.Infrastructure.Database.UnityOfWork
{
    public class DapperUnityOfWork : IUnityOfWork
    {
        private IUnitOfWorkTransaction? _currentTransaction;
        private IDbConnection _connection;

        public DapperUnityOfWork(IDbConnection dbConnection)
        {
            _connection = dbConnection;
        }

        public IUnitOfWorkTransaction? CurrentTransaction => (_currentTransaction?.Finished == false) ? _currentTransaction : null;

        public async ValueTask<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct)
        {
            if (_currentTransaction is not null)
                throw new Exception("Transacao anterior nao concluida");

            return _currentTransaction = new DapperUnityOfWorkTransaction(GetOpenConnection().BeginTransaction());
        }

        public IDbConnection GetOpenConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                return _connection;
            }
            _connection.Open();
            return _connection;
        }
    }
}