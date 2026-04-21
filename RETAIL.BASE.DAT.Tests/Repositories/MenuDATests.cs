using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class MenuItemDATests
{
    [Fact]
    public async Task GetAllAsync_WhenFilteringByEnabledAndName_ReturnsMatches()
    {
        await using var context = TestDataFactory.CreateContext();
        context.MenuItems.AddRange(
            new MenuItem { Name = "Administracion", Enabled = true },
            new MenuItem { Name = "Reportes", Enabled = true },
            new MenuItem { Name = "Administracion Legacy", Enabled = false });
        await context.SaveChangesAsync();

        var repository = new MenuItemDA(context, TestDataFactory.Logger<MenuItemDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(enabled: true, name: "Admin"))).Data.ToList();

        Assert.Single(result);
        Assert.Equal("Administracion", result[0].Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_RemovesAndReturnsTrue()
    {
        await using var context = TestDataFactory.CreateContext();
        var menuitem = new MenuItem { Name = "MenuItem A", Enabled = true };
        context.MenuItems.Add(menuitem);
        await context.SaveChangesAsync();

        var repository = new MenuItemDA(context, TestDataFactory.Logger<MenuItemDA>());

        var deleted = await repository.DeleteAsync(menuitem.Id);

        Assert.True(deleted);
        Assert.Null(await context.MenuItems.FirstOrDefaultAsync(m => m.Id == menuitem.Id));
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        await using var context = TestDataFactory.CreateContext();
        var menuitem = new MenuItem { Name = "MenuItem Detalle", Enabled = true };
        context.MenuItems.Add(menuitem);
        await context.SaveChangesAsync();

        var repository = new MenuItemDA(context, TestDataFactory.Logger<MenuItemDA>());

        var found = await repository.GetByIdAsync(menuitem.Id);

        Assert.NotNull(found);
        Assert.Equal(menuitem.Id, found!.Id);
    }
}
