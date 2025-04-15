using MediatR;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands.Requests
{
    public record AdicionarMovimentoRequest : IRequest<AdicionarMovimentoResponse>
    {
        /// <summary>
        /// Id da conta corrente
        /// </summary>
        public Guid IdContaCorrente { get; set; }
        /// <summary>
        /// id da request usado como chave de idempotencia
        /// </summary>
        public Guid IdRequest { get; set; }
        public AdicionarMovimentoDto MovimentoDto { get; set; }

    }
    public record AdicionarMovimentoDto
    {
        
        /// <summary>
        /// valor do movimento
        /// </summary>
        public decimal Valor { get; set; }
        /// <summary>
        /// Tipo do movimento digite C para Credito e D para Debito
        /// </summary>
        public char TipoMovimento { get; set; }
    }
}