using CaixaBanco.Domain.Entities;

namespace CaixaBanco.Domain.Repositories
{
    public interface IContaRepository
    {
        Task<Conta?> ObterContaAsync(string documento, CancellationToken cancellationToken);
        Task<IEnumerable<Conta>> ObterContasAsync(string nome, string documento, CancellationToken cancellationToken);
        Task<bool> CriarContaAsync(Conta conta, CancellationToken cancellationToken);
        Task<bool> InativarContaAsync(InativacaoConta inativacaoConta, CancellationToken cancellationToken);
    }
}
