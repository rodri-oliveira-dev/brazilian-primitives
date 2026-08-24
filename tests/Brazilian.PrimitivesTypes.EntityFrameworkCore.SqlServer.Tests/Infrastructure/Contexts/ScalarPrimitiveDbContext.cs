using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;

internal sealed class ScalarPrimitiveDbContext(DbContextOptions<ScalarPrimitiveDbContext> options) : DbContext(options)
{
    public DbSet<ScalarPrimitiveRecord> Records => Set<ScalarPrimitiveRecord>();

    public DbSet<EmailRecord> EmailRecords => Set<EmailRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScalarPrimitiveRecord>(entity =>
        {
            entity.ToTable("ScalarPrimitiveRecords");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Id).ValueGeneratedNever();
            entity.Property(record => record.Cpf).HasConversion(new CpfValueConverter());
            entity.Property(record => record.Cnpj).HasConversion(new CnpjValueConverter());
            entity.Property(record => record.CpfCnpj).HasConversion(new CpfCnpjValueConverter());
            entity.Property(record => record.Cep).HasConversion(new CepValueConverter());
            entity.Property(record => record.Email).HasConversion(new EmailValueConverter());
            entity.Property(record => record.OptionalEmail).HasConversion(new EmailValueConverter());
            entity.Property(record => record.MobilePhone).HasConversion(new MobilePhoneValueConverter());
            entity.Property(record => record.LandlinePhone).HasConversion(new LandlinePhoneValueConverter());
            entity.Property(record => record.TelefoneBrasileiro).HasConversion(new TelefoneBrasileiroValueConverter());
            entity.Property(record => record.ChavePix).HasConversion(new ChavePixValueConverter());
            entity.Property(record => record.Cnh).HasConversion(new CnhValueConverter());
            entity.Property(record => record.Cns).HasConversion(new CnsValueConverter());
            entity.Property(record => record.TituloEleitoral).HasConversion(new TituloEleitoralValueConverter());
            entity.Property(record => record.Nit).HasConversion(new NitValueConverter());
            entity.Property(record => record.PisPasep).HasConversion(new PisPasepValueConverter());
            entity.Property(record => record.PlacaVeiculo).HasConversion(new PlacaVeiculoValueConverter());
            entity.Property(record => record.Renavam).HasConversion(new RenavamValueConverter());
            entity.Property(record => record.Ispb).HasConversion(new IspbValueConverter());
            entity.Property(record => record.CodigoCompe).HasConversion(new CodigoCompeValueConverter());
        });

        modelBuilder.Entity<EmailRecord>(entity =>
        {
            entity.ToTable("EmailRecords");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Id).ValueGeneratedNever();
            entity.Property(record => record.Email).HasConversion(new EmailValueConverter());
        });
    }
}
