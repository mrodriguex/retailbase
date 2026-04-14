using HARD.CORE.DAT.Tests.Helpers;
using HARD.CORE.DAT.Repositories;
using HARD.CORE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace HARD.CORE.DAT.Tests.Repositories;

public class ClienteDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityIsValid_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new ClienteDA(context, TestDataFactory.Logger<ClienteDA>());
        var cliente = new Cliente { Nombre = "Cliente A", RFC = "XAXX010101000", Activo = true };

        var result = await repository.AddAsync(cliente);

        Assert.True(result > 0);
        var saved = await context.Clientes.FirstOrDefaultAsync(c => c.Id == result);
        Assert.NotNull(saved);
        Assert.Equal("Cliente A", saved!.Nombre);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByActivo_ReturnsPagedData()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Clientes.AddRange(
            new Cliente { Nombre = "A", Activo = true },
            new Cliente { Nombre = "B", Activo = false },
            new Cliente { Nombre = "C", Activo = true });
        await context.SaveChangesAsync();

        var repository = new ClienteDA(context, TestDataFactory.Logger<ClienteDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(activo: true, pageIndex: 1, pageSize: 1))).Data.ToList();

        Assert.Single(result);
        Assert.True(result[0].Activo);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new ClienteDA(context, TestDataFactory.Logger<ClienteDA>());

        var deleted = await repository.DeleteAsync(999);

        Assert.False(deleted);
    }
}
