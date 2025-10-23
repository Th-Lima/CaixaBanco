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
            if (string.IsNullOrWhiteSpace(documento))
                return null;

            var documentoNormalizado = documento.Trim().ToUpperInvariant();

            return await _db.Contas
                .FirstOrDefaultAsync(c => c.Documento.ToUpper() == documentoNormalizado);
        }

        public async Task<bool> CriarContaAsync(Conta conta)
        {
            if (conta is null) 
                throw new ArgumentNullException(nameof(conta));

            _db.Contas.Add(conta);
           
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Conta>> ObterContasAsync(string nome, string documento)
        {
            var query = _db.Contas.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(c => EF.Functions.Like(c.Nome, $"%{nome.Trim()}%"));

            if (!string.IsNullOrWhiteSpace(documento))
                query = query.Where(c => c.Documento == documento.Trim());

            return await query.ToListAsync();
        }

        public async Task<bool> InativarContaAsync(InativacaoConta inativacaoConta)
        {
            if (inativacaoConta is null) 
                throw new ArgumentNullException(nameof(inativacaoConta));
            
            _db.InativacaoContas.Add(inativacaoConta);

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
