using CaixaBanco.Application.Responses.Transacoes;
using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Enums;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using MediatR;

namespace CaixaBanco.Application.Commands.Transacoes.Transferir
{
    public class TransferirCommandHandler : IRequestHandler<TransferirCommand, TransferenciaResponse?>
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IContaRepository _contaRepository;
        private readonly INotificador _notificador;

        public TransferirCommandHandler(ITransacaoRepository transacaoRepository, 
            IContaRepository contaRepository, 
            INotificador notificador)
        {
            _transacaoRepository = transacaoRepository;
            _contaRepository = contaRepository;
            _notificador = notificador;
        }

        public async Task<TransferenciaResponse?> Handle(TransferirCommand request, CancellationToken cancellationToken)
        {
            var docOrigem = request.DocumentoOrigem.Trim();
            var docDestino = request.DocumentoDestino.Trim();

            if (docOrigem == docDestino)
            {
                _notificador.Disparar(new Notificacao("Conta de origem e destino não podem ser a mesma."));
                return null;
            }

            var origem = await _contaRepository.ObterContaAsync(docOrigem, cancellationToken);
            var destino = await _contaRepository.ObterContaAsync(docDestino, cancellationToken);

            if(!ValidacaoContas(request, origem, destino))
                return null;


            origem?.Debitar(request.Valor);
            destino?.Creditar(request.Valor);

            Transacao? transacao = null;
            try
            {
                transacao = await _transacaoRepository.ProcessarTransferenciaAsync(origem!, destino!, request.Valor, cancellationToken);

                if (transacao != null)
                {
                    return new TransferenciaResponse
                    {
                        ValorTransacao = transacao.Valor,
                        ValorContaDestinoAtualizado = destino!.Saldo,
                        ValorContaOrigemAtualizado = origem!.Saldo,
                        DocumentoDestino = destino!.Documento,
                        DocumentoOrigem = origem!.Documento,
                        CriadoEm = transacao.CriadoEm,
                    };
                }
            }
            catch (Exception ex)
            {
                _notificador.Disparar(new Notificacao($"Erro ao processar transferência: {ex.Message}"));
            }

            return null;
        }

        private bool ValidacaoContas(TransferirCommand request, Conta? origem, Conta? destino)
        {
            if (origem == null)
            {
                _notificador.Disparar(new Notificacao("Conta de origem não encontrada."));
                return false;
            }

            if (destino == null)
            {
                _notificador.Disparar(new Notificacao("Conta de destino não encontrada."));
                return false;
            }

            if (origem.Status != StatusConta.Ativa || destino.Status != StatusConta.Ativa)
            {
                _notificador.Disparar(new Notificacao("Ambas as contas devem estar ativas."));
                return false;
            }

            if (request.Valor <= 0)
            {
                _notificador.Disparar(new Notificacao("Valor da transferência deve ser positivo."));
                return false;
            }

            if (origem.Saldo < request.Valor)
            {
                _notificador.Disparar(new Notificacao("Saldo insuficiente na conta de origem."));
                return false;
            }

            return true;
        }
    }
}
