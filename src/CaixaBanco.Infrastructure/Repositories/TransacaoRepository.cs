using CaixaBanco.Data;
using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Repositories;

namespace CaixaBanco.Infrastructure.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly ApplicationDbContext _db;

        public TransacaoRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        
        public async Task<Transacao?> ProcessarTransferenciaAsync(Conta origem, Conta destino, decimal valor, CancellationToken cancellationToken)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var transacao = new Transacao(origem.Id, destino.Id, valor);
                _db.Transacoes.Add(transacao);

                _db.Contas.Update(origem);
                _db.Contas.Update(destino);

                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);

                return transacao;
            }
            catch (Exception)
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
