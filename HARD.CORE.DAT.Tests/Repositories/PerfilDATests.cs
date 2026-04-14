using HARD.CORE.DAT.Tests.Helpers;
using HARD.CORE.DAT.Repositories;
using HARD.CORE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace HARD.CORE.DAT.Tests.Repositories;

public class PerfilDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityHasExistingMenus_PersistsRelationAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var menu = new Menu { Nombre = "Dashboard", Activo = true };
        context.Menus.Add(menu);
        await context.SaveChangesAsync();

        var perfil = new Perfil
        {
            Nombre = "Administrador",
            Activo = true,
            Menus = new List<Menu> { menu }
        };

        var repository = new PerfilDA(context, TestDataFactory.Logger<PerfilDA>());

        var id = await repository.AddAsync(perfil);

        Assert.True(id > 0);
        var saved = await context.Perfiles.Include(p => p.Menus).FirstOrDefaultAsync(p => p.Id == id);
        Assert.NotNull(saved);
        Assert.Single(saved!.Menus);
        Assert.Equal(menu.Id, saved.Menus[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_LoadsMenus()
    {
        await using var context = TestDataFactory.CreateContext();
        var menu = new Menu { Nombre = "Seguridad", Activo = true };
        var perfil = new Perfil { Nombre = "Perfil 1", Menus = new List<Menu> { menu }, Activo = true };
        context.Perfiles.Add(perfil);
        await context.SaveChangesAsync();

        var repository = new PerfilDA(context, TestDataFactory.Logger<PerfilDA>());

        var found = await repository.GetByIdAsync(perfil.Id);

        Assert.NotNull(found);
        Assert.Single(found!.Menus);
        Assert.Equal("Seguridad", found.Menus[0].Nombre);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new PerfilDA(context, TestDataFactory.Logger<PerfilDA>());

        var deleted = await repository.DeleteAsync(777);

        Assert.False(deleted);
    }
}
