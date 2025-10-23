using CaixaBanco.Data;
using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CaixaBanco.Infrastructure.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly ApplicationDbContext _db;

        public ContaRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Conta?> ObterContaAsync(string documento)
        {
            var conta = await _db.Contas.FirstOrDefaultAsync(c => c.Documento == documento);

            return conta;
        }

        public async Task<bool> CriarContaAsync(Conta conta)
        {
            _db.Contas.Add(conta);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
