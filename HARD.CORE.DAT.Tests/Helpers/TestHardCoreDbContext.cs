using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HARD.CORE.DAT.Tests.Helpers;

internal sealed class TestHardCoreDbContext : HardCoreDbContext
{
    private readonly string _databaseName;

    public TestHardCoreDbContext(string databaseName)
        : base(new ConfigurationBuilder().Build())
    {
        _databaseName = databaseName;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(_databaseName);
    }
}
