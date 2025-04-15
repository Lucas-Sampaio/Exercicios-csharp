using System.Data;
using Dapper;
using Questao5.Domain.Dtos;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class IdempontenciaRepository(IDbConnection connection) : IIdempontenciaRepository
    {
        public async ValueTask Inserir(Idempotencia idempotencia)
        {
            const string sql = @"insert into idempotencia(chave_idempotencia,requisicao,resultado)
                        values (@id,@requisicao,@resultado)";

            _ = await connection.ExecuteAsync(sql, new
            {
                id = idempotencia.Id,
                requisicao = idempotencia.Requisicao,
                resultado = idempotencia.Resultado,
            });
        }

        public async ValueTask<IdempotenciaDto?> ObterPorId(Guid id)
        {
            const string sql = @"select chave_idempotencia Id, resultado from idempotencia where chave_idempotencia = @id";

            return await connection.QueryFirstOrDefaultAsync<IdempotenciaDto>(sql, new
            {
                id,
            });
        }
    }
}