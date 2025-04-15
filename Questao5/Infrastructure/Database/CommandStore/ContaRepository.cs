using System.Data;
using Dapper;
using Questao5.Domain.Dtos;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class ContaRepository(IDbConnection connection) : IContaRepository
    {
        public async ValueTask<ContaDto?> ObterPorId(Guid id)
        {
            var sql = @"select idcontacorrente id, numero, nome, ativo from contacorrente where idcontacorrente = @id";

            return await connection.QueryFirstOrDefaultAsync<ContaDto>(sql, new
            {
                id,
            });
        }
    }
}