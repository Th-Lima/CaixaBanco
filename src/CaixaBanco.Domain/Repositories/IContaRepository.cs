using CaixaBanco.Domain.Entities;

namespace CaixaBanco.Domain.Repositories
{
    public interface IContaRepository
    {
        Task<Conta?> ObterContaAsync(string documento);
        Task<bool> CriarContaAsync(Conta conta);
    }
}
