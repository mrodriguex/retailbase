using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RETAIL.BASE.DAT.Tests.Helpers;

internal sealed class TestRETAIL_BASEDbContext : RETAIL_BASEDbContext
{
    private readonly string _databaseName;

    public TestRETAIL_BASEDbContext(string databaseName)
        : base(new ConfigurationBuilder().Build())
    {
        _databaseName = databaseName;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(_databaseName);
    }
}
