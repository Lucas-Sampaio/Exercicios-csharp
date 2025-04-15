namespace Questao5.Domain.Interfaces
{
    public interface ISaldoRepository
    {
        ValueTask<decimal> ObterSaldoConta(Guid idContaCorrente);
    }
}
