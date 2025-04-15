using Questao5.Application.shared;

namespace Questao5.Application.Queries.Responses
{
    public class ObterSaldoResponse : ResponseBase
    {
        public ObterSaldoDto SaldoConta { get; set; }
    }
    public class ObterSaldoDto
    {
        public int NumeroConta { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataConsulta { get; set; }
        public decimal Saldo { get; set; }
    }
}