using CaixaBanco.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaixaBanco.Data.Mappings
{
    /// <summary>
    /// Classe de configuração da entidade InativacaoConta para o Entity Framework Core
    /// </summary>
    public class InativacaoContaConfiguration : IEntityTypeConfiguration<InativacaoConta>
    {
        public void Configure(EntityTypeBuilder<InativacaoConta> builder)
        {
            builder.ToTable("InativacaoContas");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Documento)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.UsuarioResponsavel)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.InativadoEm)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
        }
    }
}
