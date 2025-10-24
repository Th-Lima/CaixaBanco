using CaixaBanco.Application.Responses.Contas;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using MediatR;

namespace CaixaBanco.Application.Queries.Contas.ObterConta
{
    /// <summary>
    /// Classe handler para a query de obtenção de conta
    /// </summary>
    public class ObterContaQueryHandler : IRequestHandler<ObterContaQuery, IEnumerable<ContaResponse>>
    {
        private readonly IContaRepository _contaRepository;
        private readonly INotificador _notificador;

        public ObterContaQueryHandler(IContaRepository contaRepository, INotificador notificador)
        {
            _contaRepository = contaRepository;
            _notificador = notificador;
        }

        /// <summary>
        /// Método handler para manipular a query de obtenção de conta e suas regras de negócio
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ContaResponse>> Handle(ObterContaQuery request, CancellationToken cancellationToken)
        {
            var contas = await _contaRepository.ObterContasAsync(request.Nome, request.Documento, cancellationToken);

            if(contas != null && contas.Any())
            {
                return contas.Select(c => new ContaResponse
                {
                    Nome = c.Nome ?? string.Empty,
                    Documento = c.Documento ?? string.Empty,
                    Saldo = c.Saldo,
                    DataAbertura = c.DataAbertura,
                    Status = c.Status
                }).ToList();
            }

            _notificador.Disparar(new Notificacao("Não foi encontrada nenhuma conta para este documento ou nome"));
            return Enumerable.Empty<ContaResponse>();
        }
    }
}
