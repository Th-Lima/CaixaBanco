using CaixaBanco.Application.Responses.Contas;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using MediatR;

namespace CaixaBanco.Application.Queries.Contas.ObterConta
{
    public class ObterContaQueryHandler : IRequestHandler<ObterContaQuery, ContaResponse?>
    {
        private readonly IContaRepository _contaRepository;
        private readonly INotificador _notificador;

        public ObterContaQueryHandler(IContaRepository contaRepository, INotificador notificador)
        {
            _contaRepository = contaRepository;
            _notificador = notificador;
        }

        public async Task<ContaResponse?> Handle(ObterContaQuery request, CancellationToken cancellationToken)
        {
            var contas = await _contaRepository.ObterContasAsync(request.Nome, request.Documento);

            var queryReponse = contas.FirstOrDefault();

            if (queryReponse != null)
            {
                return new ContaResponse
                {
                    Nome = queryReponse.Nome ?? string.Empty,
                    Documento = queryReponse.Documento ?? string.Empty,
                    Saldo = queryReponse.Saldo,
                    DataAbertura = queryReponse.DataAbertura,
                    Status = queryReponse.Status
                };
            }

            _notificador.Disparar(new Notificacao("Não foi encontrada nenhuma conta para este documento ou nome"));
            return null;
        }
    }
}
