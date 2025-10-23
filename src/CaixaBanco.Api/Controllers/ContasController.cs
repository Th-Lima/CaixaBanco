using CaixaBanco.Application.Commands.Contas.CriarConta;
using CaixaBanco.Application.Commands.Contas.InativarConta;
using CaixaBanco.Application.Queries.Contas.ObterConta;
using CaixaBanco.Domain.Notification;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaixaBanco.Api.Controllers
{
    [Route("api/[controller]")]
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
        /// <returns></returns>
        [HttpPost]
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
