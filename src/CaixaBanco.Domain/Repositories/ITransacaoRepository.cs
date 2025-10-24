using CaixaBanco.Domain.Entities;

namespace CaixaBanco.Domain.Repositories
{
    public interface ITransacaoRepository
    {
        Task<Transacao?> ProcessarTransferenciaAsync(Conta origem, Conta destino, decimal valor, CancellationToken cancellationToken);
    }
}
