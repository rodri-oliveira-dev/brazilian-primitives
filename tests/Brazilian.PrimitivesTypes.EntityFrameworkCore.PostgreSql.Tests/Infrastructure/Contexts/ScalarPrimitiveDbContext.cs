using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class ScalarPrimitiveDbContext(DbContextOptions<ScalarPrimitiveDbContext> options) : DbContext(options)
{
    public DbSet<ScalarPrimitiveRecord> Records => Set<ScalarPrimitiveRecord>();

    public DbSet<EmailRecord> EmailRecords => Set<EmailRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");

        modelBuilder.Entity<ScalarPrimitiveRecord>(entity =>
        {
            entity.ToTable("scalar_primitive_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Id).HasColumnName("id").ValueGeneratedNever();
            ScalarPrimitivePostgreSqlMappings.Cpf.Apply(entity.Property(record => record.Cpf).HasColumnName("cpf"));
            ScalarPrimitivePostgreSqlMappings.Cnpj.Apply(entity.Property(record => record.Cnpj).HasColumnName("cnpj"));
            ScalarPrimitivePostgreSqlMappings.CpfCnpj.Apply(entity.Property(record => record.CpfCnpj).HasColumnName("cpf_cnpj"));
            ScalarPrimitivePostgreSqlMappings.Cep.Apply(entity.Property(record => record.Cep).HasColumnName("cep"));
            ScalarPrimitivePostgreSqlMappings.Email.Apply(entity.Property(record => record.Email).HasColumnName("email"));
            ScalarPrimitivePostgreSqlMappings.Email.Apply(entity.Property(record => record.OptionalEmail).HasColumnName("optional_email"));
            ScalarPrimitivePostgreSqlMappings.MobilePhone.Apply(entity.Property(record => record.MobilePhone).HasColumnName("mobile_phone"));
            ScalarPrimitivePostgreSqlMappings.LandlinePhone.Apply(entity.Property(record => record.LandlinePhone).HasColumnName("landline_phone"));
            ScalarPrimitivePostgreSqlMappings.TelefoneBrasileiro.Apply(entity.Property(record => record.TelefoneBrasileiro).HasColumnName("telefone_brasileiro"));
            ScalarPrimitivePostgreSqlMappings.ChavePix.Apply(entity.Property(record => record.ChavePix).HasColumnName("chave_pix"));
            ScalarPrimitivePostgreSqlMappings.Cnh.Apply(entity.Property(record => record.Cnh).HasColumnName("cnh"));
            ScalarPrimitivePostgreSqlMappings.Cns.Apply(entity.Property(record => record.Cns).HasColumnName("cns"));
            ScalarPrimitivePostgreSqlMappings.TituloEleitoral.Apply(entity.Property(record => record.TituloEleitoral).HasColumnName("titulo_eleitoral"));
            ScalarPrimitivePostgreSqlMappings.Nit.Apply(entity.Property(record => record.Nit).HasColumnName("nit"));
            ScalarPrimitivePostgreSqlMappings.PisPasep.Apply(entity.Property(record => record.PisPasep).HasColumnName("pis_pasep"));
            ScalarPrimitivePostgreSqlMappings.PlacaVeiculo.Apply(entity.Property(record => record.PlacaVeiculo).HasColumnName("placa_veiculo"));
            ScalarPrimitivePostgreSqlMappings.Renavam.Apply(entity.Property(record => record.Renavam).HasColumnName("renavam"));
            ScalarPrimitivePostgreSqlMappings.Ispb.Apply(entity.Property(record => record.Ispb).HasColumnName("ispb"));
            ScalarPrimitivePostgreSqlMappings.CodigoCompe.Apply(entity.Property(record => record.CodigoCompe).HasColumnName("codigo_compe"));
        });

        modelBuilder.Entity<EmailRecord>(entity =>
        {
            entity.ToTable("email_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Id).HasColumnName("id").ValueGeneratedNever();
            ScalarPrimitivePostgreSqlMappings.Email.Apply(entity.Property(record => record.Email).HasColumnName("email"));
        });
    }
}
