using CaixaBanco.Application.Commands.Contas.CriarConta;
using CaixaBanco.Application.Commands.Contas.InativarConta;
using CaixaBanco.Application.Queries.Contas.ObterConta;
using CaixaBanco.Application.Responses.Contas;
using CaixaBanco.Domain.Notification;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaixaBanco.Api.Controllers
{
    [Route("[controller]")]
    public class ContasController : MainController
    {
        private readonly IMediator _mediator;

        public ContasController(INotificador notificador, IMediator mediator) : base(notificador)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Endpoint para criar conta bancária
        /// </summary>
        /// <param name="criarContacommand"></param>
        /// <response code="200">Retorna booleano de acordo com o sucesso da operação de criar conta.</response>
        /// <response code="400">Se ocorrer um erro na criação da conta</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Criar([FromBody] CriarContaCommand criarContacommand)
        {
            if (!ModelState.IsValid) 
                return ResponseCustomizado(ModelState);
                
            var resultado =  await _mediator.Send(criarContacommand);

            return ResponseCustomizado(resultado);
        }

        /// <summary>
        /// Endpoint para obter conta bancária por nome ou documento
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Obter([FromQuery] string? nome, [FromQuery] string? documento)
        {
            var obterContaQuery = new ObterContaQuery
            {
                Nome = nome ?? string.Empty,
                Documento = documento ?? string.Empty
            };

            var resultado = await _mediator.Send(obterContaQuery);

            return ResponseCustomizado(resultado);
        }

        /// <summary>
        /// Endpoint para inativar conta bancária
        /// </summary>
        /// <param name="inativarContaCommand"></param>
        /// <returns></returns>
        [HttpPatch("/inativar")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Inativar([FromBody] InativarContaCommand inativarContaCommand)
        {
            if (!ModelState.IsValid)
                return ResponseCustomizado(ModelState);

            if (string.IsNullOrWhiteSpace(inativarContaCommand.Documento))
            {
                NotificacaoErro("Documento da conta a ser inativa, deve ser informado corretamente");
                return ResponseCustomizado();
            }

            var resultado = await _mediator.Send(inativarContaCommand);
            return ResponseCustomizado(resultado);
        }
    }
}
