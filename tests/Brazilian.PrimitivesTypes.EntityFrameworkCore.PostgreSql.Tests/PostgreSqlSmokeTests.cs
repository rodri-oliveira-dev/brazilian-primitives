using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests;

[Collection(PostgreSqlTestCollection.Name)]
public sealed class PostgreSqlSmokeTests
{
    private const string SmokeDatabaseName = "brazilian_primitives_smoke_tests";

    private readonly PostgreSqlContainerFixture _fixture;

    public PostgreSqlSmokeTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task EfCoreCanCreateAndUsePostgreSqlDatabase()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string connectionString = _fixture.GetConnectionString(SmokeDatabaseName);
        DbContextOptions<SmokeDbContext> options = new DbContextOptionsBuilder<SmokeDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using SmokeDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            bool created = await context.Database.EnsureCreatedAsync(cancellationToken);

            Assert.True(created);

            context.Records.Add(new SmokeRecord { Value = "postgresql-ready" });
            await context.SaveChangesAsync(cancellationToken);

            string persistedValue = await context.Records
                .AsNoTracking()
                .Select(record => record.Value)
                .SingleAsync(cancellationToken);

            Assert.Equal("postgresql-ready", persistedValue);
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    private sealed class SmokeDbContext(DbContextOptions<SmokeDbContext> options) : DbContext(options)
    {
        public DbSet<SmokeRecord> Records => Set<SmokeRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SmokeRecord>(entity =>
            {
                entity.ToTable("smoke_records");
                entity.HasKey(record => record.Id);
                entity.Property(record => record.Value)
                    .HasColumnName("value")
                    .HasMaxLength(64)
                    .IsRequired();
            });
        }
    }

    private sealed class SmokeRecord
    {
        public int Id
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        } = string.Empty;
    }
}
