using CaixaBanco.Application.Commands.Transacoes.Transferir;
using CaixaBanco.Application.Responses.Transacoes;
using CaixaBanco.Domain.Notification;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaixaBanco.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciar ações de transações entre as contas
    /// </summary>
    [Route("[controller]")]
    public class TransacoesController : MainController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Construtor da classe TransacoesController para injeção de dependências e inicialização.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="notificador"></param>
        public TransacoesController(IMediator mediator, INotificador notificador) : base(notificador)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Endpoint para transferir valor entre contas
        /// </summary>
        /// <param name="transferirCommand">Payload a ser enviado para transferÊncias entre contas</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TransferenciaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Transferir([FromBody] TransferirCommand transferirCommand, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) 
                return ResponseCustomizado(ModelState);

            var resultado = await _mediator.Send(new TransferirCommand
            {
                DocumentoOrigem = transferirCommand.DocumentoOrigem,
                DocumentoDestino = transferirCommand.DocumentoDestino,
                Valor = transferirCommand.Valor
            }, cancellationToken);

            return ResponseCustomizado(resultado);
        }
    }
}
