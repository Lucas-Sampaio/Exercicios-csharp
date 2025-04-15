//using System.Net;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using Questao5.Application.Commands.Requests;
//using Questao5.Domain.Errors;

//namespace Questao5.Infrastructure.Services.Controllers
//{
//    [Route("api/[controller]")]
//    public class MovimentoController(IMediator mediator) : Controller
//    {
//        /// <summary>
//        /// Cadastra um movimento
//        /// </summary>
//        /// <param name="request">informacoes da movimentacao para cadastro</param>
//        /// <param name="ct"></param>
//        /// <returns>retorna o id da movimentacao</returns>
//        [HttpPost("")]
//        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
//        [ProducesResponseType(typeof(List<Error>), (int)HttpStatusCode.BadRequest)]
//        public async Task<IActionResult> Post(AdicionarMovimentoRequest request, CancellationToken ct)
//        {
//            var response = await mediator.Send(request, ct);
//            if (response.Errors.Count > 0)
//                return BadRequest(response.Errors);

//            return Ok(response.Id);
//        }
//    }
//}