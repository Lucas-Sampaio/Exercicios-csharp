using FluentValidation;
using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain;
using Questao5.Domain.Errors;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Handlers
{
    public class ObterSaldoHandler(
        IValidator<ObterSaldoRequest> validator,
        IContaRepository contaRepository,
        ISaldoRepository saldoRepository) : IRequestHandler<ObterSaldoRequest, ObterSaldoResponse>
    {
        public async Task<ObterSaldoResponse> Handle(ObterSaldoRequest request, CancellationToken cancellationToken)
        {
            var response = new ObterSaldoResponse();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.AdicionarErros(validationResult.Errors);
                return response;
            }

            var conta = await contaRepository.ObterPorId(request.IdContaCorrente);

            if (conta is null)
            {
                var error = new Error(nameof(ErrorMessages.INVALID_ACCOUNT), ErrorMessages.INVALID_ACCOUNT);
                response.AdicionarErros(error);
                return response;
            }

            if (!conta.Ativo)
            {
                var error = new Error(nameof(ErrorMessages.INACTIVE_ACCOUNT), ErrorMessages.INACTIVE_ACCOUNT);
                response.AdicionarErros(error);
                return response;
            }

            response.SaldoConta = new ObterSaldoDto
            {
                Saldo = await saldoRepository.ObterSaldoConta(request.IdContaCorrente),
                DataConsulta = DateTime.Now,
                NomeTitular = conta.Nome,
                NumeroConta = conta.Numero
            };

            return response;
        }
    }
}