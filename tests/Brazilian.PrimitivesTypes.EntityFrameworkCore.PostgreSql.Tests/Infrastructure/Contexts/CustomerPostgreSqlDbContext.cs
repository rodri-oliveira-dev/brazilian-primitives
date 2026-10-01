using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class CustomerPostgreSqlDbContext(
    DbContextOptions<CustomerPostgreSqlDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");

        var entity = modelBuilder.Entity<Customer>();
        entity.ToTable("customers");
        entity.HasKey(customer => customer.Id);
        entity.Property(customer => customer.Id).HasColumnName("id").ValueGeneratedNever();
        entity.Property(customer => customer.Cpf).HasColumnName("cpf");
        entity.Property(customer => customer.Email).HasColumnName("email");
        entity.Property(customer => customer.Cep).HasColumnName("cep");
    }
}
