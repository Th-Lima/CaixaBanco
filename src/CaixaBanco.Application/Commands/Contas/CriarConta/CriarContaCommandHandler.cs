using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using MediatR;

namespace CaixaBanco.Application.Commands.Contas.CriarConta
{
    public class CriarContaCommandHandler : IRequestHandler<CriarContaCommand, bool>
    {
        private readonly IContaRepository _contaRepository;
        private readonly INotificador _notificador;

        public CriarContaCommandHandler(IContaRepository contaRepository, INotificador notificador)
        {
            _contaRepository = contaRepository;
            _notificador = notificador;
        }

        public async Task<bool> Handle(CriarContaCommand command, CancellationToken cancellationToken)
        {
            var documento = command.Documento?.Trim();

            if (string.IsNullOrWhiteSpace(documento) || string.IsNullOrWhiteSpace(command.Nome))
            {
                _notificador.Disparar(new Notificacao("Documento e Nome precisam ser enviados para criação da conta"));
                return false;
            }

            var contaDb = await _contaRepository.ObterContaAsync(documento!, cancellationToken);
            if (contaDb != null)
            {
                _notificador.Disparar(new Notificacao("Já existe uma conta cadastrada para este documento."));
                return false;
            }
            else
            {
                var conta = new Conta(command.Nome, documento!);
               
                var resultado = await _contaRepository.CriarContaAsync(conta, cancellationToken);
                return resultado;
            }
        }
    }
}
