using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Errors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Questao5.Infrastructure.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContaController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Cadastra um movimento
        /// </summary>
        /// <param name="idConta">id da conta onde sera realizada a movimentacao</param>
        /// <param name="IdRequest">id da chave de idempotencia passada no header</param>
        /// <param name="movimento">informacoes da movimentacao para cadastro</param>
        /// <param name="ct"></param>
        /// <returns>retorna o id da movimentacao</returns>
        [HttpPost("{idConta}/movimento")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<Error>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(
            [FromRoute] Guid idConta,
            [FromHeader] Guid IdRequest,
            [FromBody] AdicionarMovimentoDto movimento,
            CancellationToken ct)
        {
            var command = new AdicionarMovimentoRequest
            {
                IdContaCorrente = idConta,
                IdRequest = IdRequest,
                MovimentoDto = movimento
            };

            var response = await mediator.Send(command, ct);
            if (response.Errors.Count > 0)
                return BadRequest(response.Errors);

            return Ok(response.Id);
        }

        /// <summary>
        /// Obten informacoes da conta com saldo atraves do id da conta
        /// </summary>
        /// <param name="idConta">id da conta para consulta</param>
        /// <param name="ct"></param>
        /// <returns>retorna informacoes da conta com saldo</returns>
        [HttpGet("{idConta}/saldo")]
        [ProducesResponseType(typeof(ObterSaldoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<Error>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get([FromRoute] Guid idConta, CancellationToken ct)
        {
            var command = new ObterSaldoRequest
            {
                IdContaCorrente = idConta,
            };

            var response = await mediator.Send(command, ct);
            if (response.Errors.Count > 0)
                return BadRequest(response.Errors);

            return Ok(response.SaldoConta);
        }
    }
}