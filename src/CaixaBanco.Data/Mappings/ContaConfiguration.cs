using CaixaBanco.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaixaBanco.Data.Mappings
{
    public class ContaConfiguration : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.ToTable("Contas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Documento)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.Documento)
                .IsUnique();

            builder.Property(x => x.Saldo)
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasDefaultValue(1000m);

            builder.Property(x => x.DataAbertura)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.VersaoLinha)
                .IsRowVersion();
        }
    }
}
