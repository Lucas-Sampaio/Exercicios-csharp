using Questao5.Domain.Dtos;
using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IContaRepository
    {
        ValueTask<ContaDto?> ObterPorId(Guid id);
    }
}
