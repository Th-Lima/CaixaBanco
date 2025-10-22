using CaixaBanco.Data.Mappings;
using CaixaBanco.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaixaBanco.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Conta> Contas => Set<Conta>();
        public DbSet<Transacao> Transacoes => Set<Transacao>();
        public DbSet<InativacaoConta> InativacaoContas => Set<InativacaoConta>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ContaConfiguration());
            modelBuilder.ApplyConfiguration(new TransacaoConfiguration());
            modelBuilder.ApplyConfiguration(new InativacaoContaConfiguration());

            // Ajustes específicos para SQLite (substitui GETUTCDATE() por CURRENT_TIMESTAMP)
            var provider = Database.ProviderName ?? string.Empty;
            if (provider.Contains("Sqlite"))
            {
                modelBuilder.Entity<Conta>().Property(c => c.DataAbertura)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                modelBuilder.Entity<Transacao>().Property(t => t.CriadoEm)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                modelBuilder.Entity<InativacaoConta>().Property(i => i.InativadoEm)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
