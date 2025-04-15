using Questao5.Domain.Entities;

namespace Questao5.Domain.Repositories
{
    public interface IMovimentoRepository
    {
        ValueTask<Guid> Inserir(Movimento movimento);
    }
}
