using CaixaBanco.Application.Commands.Contas.CriarConta;
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
    }
}
