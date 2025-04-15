using System.Data;
using Dapper;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public class SaldoRepository(IDbConnection connection) : ISaldoRepository
    {
        public async ValueTask<decimal> ObterSaldoConta(Guid idContaCorrente)
        {
            var sql = @"select SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END) -
                    SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END)
                    from Movimento where idcontacorrente = @id";

            var saldo = await connection.QueryFirstOrDefaultAsync<decimal?>(sql, new
            {
                id = idContaCorrente,
            });
            return saldo.GetValueOrDefault();
        }
    }
}