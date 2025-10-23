using CaixaBanco.Domain.Entities;

namespace CaixaBanco.Domain.Repositories
{
    public interface IContaRepository
    {
        Task<Conta?> ObterContaAsync(string documento);
        Task<IEnumerable<Conta>> ObterContasAsync(string nome, string documento);
        Task<bool> CriarContaAsync(Conta conta);
        Task<bool> InativarContaAsync(InativacaoConta inativacaoConta);
    }
}
