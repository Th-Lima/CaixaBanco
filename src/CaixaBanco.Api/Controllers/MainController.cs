using CaixaBanco.Domain.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CaixaBanco.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador _notificador;

        public Guid UserId { get; set; }
        protected bool AuthenticatedUser { get; set; }

        protected MainController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool ValidaOperacao()
        {
            return !_notificador.TemNotificacoes();
        }

        protected ActionResult ResponseCustomizado(object resultado = null)
        {
            if (ValidaOperacao())
                return Ok(new
                {
                    success = true,
                    data = resultado
                });

            return BadRequest(new
            {
                success = false,
                errors = _notificador.ObterNotificacoes().Select(x => x.Mensagem)
            });
        }

        protected ActionResult ResponseCustomizado(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
                NotificacaoErroModelStateInvalido(modelState);

            return ResponseCustomizado();
        }

        protected void NotificacaoErroModelStateInvalido(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(x => x.Errors);

            foreach (var erro in erros)
            {
                var errorMessage = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;

                NotificacaoErro(errorMessage);
            }
        }

        protected void NotificacaoErro(string message)
        {
            _notificador.Disparar(new Notificacao(message));
        }
    }
}
