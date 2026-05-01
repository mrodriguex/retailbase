using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class RoleDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityHasExistingMenuItems_PersistsRelationAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var menuitem = new MenuItem { Name = "Dashboard", Enabled = true };
        context.MenuItems.Add(menuitem);
        await context.SaveChangesAsync();

        var role = new Role
        {
            Name = "Administrador",
            Enabled = true,
            MenuItems = new List<MenuItem> { menuitem }
        };

        var repository = new RoleDA(context, TestDataFactory.Logger<RoleDA>());

        var id = await repository.AddAsync(role);

        Assert.True(id > 0);
        var saved = await context.Roles.Include(p => p.MenuItems).FirstOrDefaultAsync(p => p.Id == id);
        Assert.NotNull(saved);
        Assert.Single(saved!.MenuItems);
        Assert.Equal(menuitem.Id, saved.MenuItems[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_LoadsMenuItems()
    {
        await using var context = TestDataFactory.CreateContext();
        var menuitem = new MenuItem { Name = "Seguridad", Enabled = true };
        var role = new Role { Name = "Role 1", MenuItems = new List<MenuItem> { menuitem }, Enabled = true };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var repository = new RoleDA(context, TestDataFactory.Logger<RoleDA>());

        var found = await repository.GetByIdAsync(role.Id);

        Assert.NotNull(found);
        Assert.Single(found!.MenuItems);
        Assert.Equal("Seguridad", found.MenuItems[0].Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new RoleDA(context, TestDataFactory.Logger<RoleDA>());

        var deleted = await repository.DeleteAsync(777);

        Assert.False(deleted);
    }
}
