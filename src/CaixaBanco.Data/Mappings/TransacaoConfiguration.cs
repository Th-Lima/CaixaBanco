using CaixaBanco.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaixaBanco.Data.Mappings
{
    public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.ToTable("Transacoes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Valor)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.CriadoEm)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.HasOne(x => x.ContaOrigem)
                .WithMany(a => a.Transacoes)
                .HasForeignKey(x => x.ContaOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ContaDestino)
                .WithMany()
                .HasForeignKey(x => x.ContaDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
