using System.Text.Json;
using FluentValidation;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Errors;
using Questao5.Domain.Interfaces;
using Questao5.Domain.Repositories;

namespace Questao5.Application.Handlers
{
    public class AdicionarMovimentoHandler(
        IValidator<AdicionarMovimentoRequest> validator,
        IContaRepository contaRepository,
        IIdempontenciaRepository idempontenciaRepository,
        IMovimentoRepository movimentoRepository) : IRequestHandler<AdicionarMovimentoRequest, AdicionarMovimentoResponse>
    {
        public async Task<AdicionarMovimentoResponse> Handle(AdicionarMovimentoRequest request, CancellationToken cancellationToken)
        {
            var validationResult = validator.Validate(request);
            var response = new AdicionarMovimentoResponse();

            if (!validationResult.IsValid)
            {
                response.AdicionarErros(validationResult.Errors);
                return response;
            }

            var idemponteciaDto = await idempontenciaRepository.ObterPorId(request.IdRequest);
            if (idemponteciaDto is not null)
                return JsonSerializer.Deserialize<AdicionarMovimentoResponse>(idemponteciaDto.Resultado);

            if (await ValidarConta(request.IdContaCorrente)
                is var erro && erro is not null)
            {
                response.AdicionarErros(erro);
                return response;
            }

            var movimentoDto = request.MovimentoDto;

            var movimento = new Movimento(request.IdContaCorrente, movimentoDto.Valor, (ETipoMovimento)movimentoDto.TipoMovimento);
            response.Id = await movimentoRepository.Inserir(movimento);

            var requestString = JsonSerializer.Serialize(request);
            var responseString = JsonSerializer.Serialize(response);
            var idempontecia = new Idempotencia(request.IdRequest, requestString, responseString);

            await idempontenciaRepository.Inserir(idempontecia);
            return response;
        }

        private async ValueTask<Error?> ValidarConta(Guid idConta)
        {
            var conta = await contaRepository.ObterPorId(idConta);
            if (conta is null)
                return new Error(nameof(ErrorMessages.INVALID_ACCOUNT), ErrorMessages.INVALID_ACCOUNT);

            if (!conta.Ativo)
                return new Error(nameof(ErrorMessages.INACTIVE_ACCOUNT), ErrorMessages.INACTIVE_ACCOUNT);

            return null;
        }
    }
}