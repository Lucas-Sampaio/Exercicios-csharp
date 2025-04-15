using System.Data;
using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Repositories;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class MovimentoRepository(IDbConnection connection) : IMovimentoRepository
    {
        public async ValueTask<Guid> Inserir(Movimento movimento)
        {
            var sql = @"insert into Movimento(idmovimento,idcontacorrente,datamovimento,tipomovimento,valor)
                        values (@id,@idcontacorrente,@datamovimento,@tipomovimento,@valor)";

            _ = await connection.ExecuteAsync(sql, new
            {
                id = movimento.Id,
                idcontacorrente = movimento.IdContaCorrente,
                datamovimento = movimento.DataMovimento.ToString("dd/MM/yyyy"),
                tipomovimento = (char)movimento.TipoMovimento,
                valor = Math.Round(movimento.Valor, 2)
            });

            return movimento.Id;
        }
    }
}