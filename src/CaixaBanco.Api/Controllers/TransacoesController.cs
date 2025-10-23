using CaixaBanco.Application.Commands.Transacoes.Transferir;
using CaixaBanco.Application.Responses.Transacoes;
using CaixaBanco.Domain.Notification;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaixaBanco.Api.Controllers
{
    [Route("[controller]")]
    public class TransacoesController : MainController
    {
        private readonly IMediator _mediator;

        public TransacoesController(IMediator mediator, INotificador notificador) : base(notificador)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Endpoint para transferir valor entre contas
        /// </summary>
        /// <param name="transferirCommand"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TransferenciaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Transferir([FromBody] TransferirCommand transferirCommand)
        {
            if (!ModelState.IsValid) 
                return ResponseCustomizado(ModelState);

            var resultado = await _mediator.Send(new TransferirCommand
            {
                DocumentoOrigem = transferirCommand.DocumentoOrigem,
                DocumentoDestino = transferirCommand.DocumentoDestino,
                Valor = transferirCommand.Valor
            });

            return ResponseCustomizado(resultado);
        }
    }
}
