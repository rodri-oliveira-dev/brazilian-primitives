using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests;

[Collection(SqlServerTestCollection.Name)]
public sealed class SqlServerSmokeTests
{
    private const string SmokeDatabaseName = "BrazilianPrimitivesSmokeTests";

    private readonly SqlServerContainerFixture _fixture;

    public SqlServerSmokeTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task EfCoreCanCreateAndUseSqlServerDatabase()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string connectionString = _fixture.GetConnectionString(SmokeDatabaseName);
        DbContextOptions<SmokeDbContext> options = new DbContextOptionsBuilder<SmokeDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using SmokeDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            bool created = await context.Database.EnsureCreatedAsync(cancellationToken);

            Assert.True(created);

            context.Records.Add(new SmokeRecord { Value = "sql-server-ready" });
            await context.SaveChangesAsync(cancellationToken);

            string persistedValue = await context.Records
                .AsNoTracking()
                .Select(record => record.Value)
                .SingleAsync(cancellationToken);

            Assert.Equal("sql-server-ready", persistedValue);
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
                entity.ToTable("SmokeRecords");
                entity.HasKey(record => record.Id);
                entity.Property(record => record.Value)
                    .HasMaxLength(64)
                    .IsUnicode(false)
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
