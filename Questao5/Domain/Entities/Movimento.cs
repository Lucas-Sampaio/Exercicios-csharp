using Questao5.Domain.Enumerators;

namespace Questao5.Domain.Entities
{
    public class Movimento(Guid idContaCorrente, decimal valor, ETipoMovimento tipoMovimento) : Entity
    {
        public Guid IdContaCorrente { get; } = idContaCorrente;
        public DateTime DataMovimento { get; } = DateTime.Today;
        public ETipoMovimento TipoMovimento { get; } = tipoMovimento;
        public decimal Valor { get; } = valor;
    }
}