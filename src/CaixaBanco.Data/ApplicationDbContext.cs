using CaixaBanco.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CaixaBanco.Data
{
    /// <summary>
    /// Classe de contexto do banco de dados para o Entity Framework Core
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Conta> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<InativacaoConta> InativacaoContas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
