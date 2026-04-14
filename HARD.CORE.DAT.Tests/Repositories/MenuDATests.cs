using HARD.CORE.DAT.Tests.Helpers;
using HARD.CORE.DAT.Repositories;
using HARD.CORE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace HARD.CORE.DAT.Tests.Repositories;

public class MenuDATests
{
    [Fact]
    public async Task GetAllAsync_WhenFilteringByActivoAndNombre_ReturnsMatches()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Menus.AddRange(
            new Menu { Nombre = "Administracion", Activo = true },
            new Menu { Nombre = "Reportes", Activo = true },
            new Menu { Nombre = "Administracion Legacy", Activo = false });
        await context.SaveChangesAsync();

        var repository = new MenuDA(context, TestDataFactory.Logger<MenuDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(activo: true, nombre: "Admin"))).Data.ToList();

        Assert.Single(result);
        Assert.Equal("Administracion", result[0].Nombre);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_RemovesAndReturnsTrue()
    {
        await using var context = TestDataFactory.CreateContext();
        var menu = new Menu { Nombre = "Menu A", Activo = true };
        context.Menus.Add(menu);
        await context.SaveChangesAsync();

        var repository = new MenuDA(context, TestDataFactory.Logger<MenuDA>());

        var deleted = await repository.DeleteAsync(menu.Id);

        Assert.True(deleted);
        Assert.Null(await context.Menus.FirstOrDefaultAsync(m => m.Id == menu.Id));
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        await using var context = TestDataFactory.CreateContext();
        var menu = new Menu { Nombre = "Menu Detalle", Activo = true };
        context.Menus.Add(menu);
        await context.SaveChangesAsync();

        var repository = new MenuDA(context, TestDataFactory.Logger<MenuDA>());

        var found = await repository.GetByIdAsync(menu.Id);

        Assert.NotNull(found);
        Assert.Equal(menu.Id, found!.Id);
    }
}
