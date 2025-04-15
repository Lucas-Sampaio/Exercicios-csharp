using Questao5.Domain.Dtos;
using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IIdempontenciaRepository
    {
        ValueTask<IdempotenciaDto?> ObterPorId(Guid id);
        ValueTask Inserir(Idempotencia idempotencia);
    }
}
