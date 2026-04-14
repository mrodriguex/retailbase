using HARD.CORE.OBJ;
using Microsoft.Extensions.Logging;

namespace HARD.CORE.DAT.Tests.Helpers;

internal static class TestDataFactory
{
    public static TestHardCoreDbContext CreateContext(string? databaseName = null)
    {
        return new TestHardCoreDbContext(databaseName ?? Guid.NewGuid().ToString("N"));
    }

    public static ILogger<T> Logger<T>()
    {
        return LoggerFactory.Create(builder => { }).CreateLogger<T>();
    }

    public static BaseFilter Paged(bool? activo = null, string nombre = "", int pageIndex = 1, int pageSize = 50)
    {
        return new BaseFilter
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Activo = activo,
            Nombre = nombre
        };
    }
}
