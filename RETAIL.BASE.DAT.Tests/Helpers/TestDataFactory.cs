using RETAIL.BASE.OBJ;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.DAT.Tests.Helpers;

internal static class TestDataFactory
{
    public static TestRETAIL_BASEDbContext CreateContext(string? databaseName = null)
    {
        return new TestRETAIL_BASEDbContext(databaseName ?? Guid.NewGuid().ToString("N"));
    }

    public static ILogger<T> Logger<T>()
    {
        return LoggerFactory.Create(builder => { }).CreateLogger<T>();
    }

    public static BaseFilter Paged(bool? enabled = null, string name = "", int pageIndex = 1, int pageSize = 50)
    {
        return new BaseFilter
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Enabled = enabled,
            Name = name
        };
    }
}
