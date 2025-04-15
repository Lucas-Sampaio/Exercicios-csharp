using MediatR;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{
    public class ObterSaldoRequest : IRequest<ObterSaldoResponse>
    {
        public Guid IdContaCorrente { get; set; }
    }
}
