using CaixaBanco.Application.Commands.Contas.CriarConta;
using CaixaBanco.Application.Commands.Contas.InativarConta;
using CaixaBanco.Application.Queries.Contas.ObterConta;
using CaixaBanco.Application.Responses.Contas;
using CaixaBanco.Domain.Notification;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaixaBanco.Api.Controllers
{
    /// <summary>
    /// Controlller para gerenciar ações das contas bancárias
    /// </summary>
    [Route("[controller]")]
    public class ContasController : MainController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Construtor da classe ContasController para injeção de dependências e inicialização.
        /// </summary>
        /// <param name="notificador"></param>
        /// <param name="mediator"></param>
        public ContasController(INotificador notificador, IMediator mediator) : base(notificador)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Endpoint para criar conta bancária
        /// </summary>
        /// <param name="criarContacommand">Payload que deverá ser enviado para criação da conta.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Criar([FromBody] CriarContaCommand criarContacommand, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) 
                return ResponseCustomizado(ModelState);
                
            var resultado =  await _mediator.Send(criarContacommand, cancellationToken);

            return ResponseCustomizado(resultado);
        }

        /// <summary>
        /// Endpoint para obter conta bancária por nome ou documento
        /// </summary>
        /// <param name="nome">Parametro string 'nome' para obter a conta.</param>
        /// <param name="documento">Parametro string 'documento' para obter a conta.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Obter([FromQuery] string? nome, [FromQuery] string? documento, CancellationToken cancellationToken)
        {
            var obterContaQuery = new ObterContaQuery
            {
                Nome = nome ?? string.Empty,
                Documento = documento ?? string.Empty
            };

            var resultado = await _mediator.Send(obterContaQuery, cancellationToken);

            return ResponseCustomizado(resultado);
        }

        /// <summary>
        /// Endpoint para inativar conta bancária
        /// </summary>
        /// <param name="inativarContaCommand">Payload a ser enviado para inativar uma conta</param>
        /// <returns></returns>
        [HttpPatch("/inativar")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Inativar([FromBody] InativarContaCommand inativarContaCommand, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ResponseCustomizado(ModelState);

            if (string.IsNullOrWhiteSpace(inativarContaCommand.Documento))
            {
                NotificacaoErro("Documento da conta a ser inativa, deve ser informado corretamente");
                return ResponseCustomizado();
            }

            var resultado = await _mediator.Send(inativarContaCommand, cancellationToken);
            return ResponseCustomizado(resultado);
        }
    }
}
