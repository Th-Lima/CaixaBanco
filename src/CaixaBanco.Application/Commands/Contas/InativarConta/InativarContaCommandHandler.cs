using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Enums;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using MediatR;

namespace CaixaBanco.Application.Commands.Contas.InativarConta
{
    /// <summary>
    /// Classe handler para o comando de inativação de conta
    /// </summary>
    public class InativarContaCommandHandler : IRequestHandler<InativarContaCommand, bool>
    {
        private readonly IContaRepository _contaRepository;
        private readonly INotificador _notificador;

        public InativarContaCommandHandler(IContaRepository contaRepository, INotificador notificador)
        {
            _contaRepository = contaRepository;
            _notificador = notificador;
        }

        /// <summary>
        /// Método handler para manipular o comando de inativação de conta e suas regras de negócio
        /// </summary>
        /// <param name="inativarContaCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(InativarContaCommand inativarContaCommand, CancellationToken cancellationToken)
        {
            var conta = await _contaRepository.ObterContaAsync(inativarContaCommand.Documento!.Trim(), cancellationToken);
            if (conta == null)
            {
                _notificador.Disparar(new Notificacao("Conta não encontrada."));
                return false;
            }

            if (conta.Status != StatusConta.Ativa)
            {
                _notificador.Disparar(new Notificacao("Conta já está inativa."));
                return false;
            }

            conta.Inativar();

            var usuario = string.IsNullOrWhiteSpace(inativarContaCommand.UsuarioResponsavel) 
                ? "system" 
                : inativarContaCommand.UsuarioResponsavel;

            var registro = new InativacaoConta(conta.Documento, usuario);

            return await _contaRepository.InativarContaAsync(registro, cancellationToken);
        }
    }
}
