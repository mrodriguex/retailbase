using HARD.CORE.DAT.Tests.Helpers;
using HARD.CORE.DAT.Repositories;
using HARD.CORE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace HARD.CORE.DAT.Tests.Repositories;

public class EmpresaDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityIsValid_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new EmpresaDA(context, TestDataFactory.Logger<EmpresaDA>());
        var empresa = new Empresa { Nombre = "Empresa A", RFC = "RFC01", Activo = true };

        var result = await repository.AddAsync(empresa);

        Assert.True(result > 0);
        var saved = await context.Empresas.FirstOrDefaultAsync(e => e.Id == result);
        Assert.NotNull(saved);
        Assert.Equal("Empresa A", saved!.Nombre);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByActivo_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Empresas.AddRange(
            new Empresa { Nombre = "A", Activo = true },
            new Empresa { Nombre = "B", Activo = false });
        await context.SaveChangesAsync();

        var repository = new EmpresaDA(context, TestDataFactory.Logger<EmpresaDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(activo: false))).Data.ToList();

        Assert.Single(result);
        Assert.False(result[0].Activo);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_RemovesAndReturnsTrue()
    {
        await using var context = TestDataFactory.CreateContext();
        var empresa = new Empresa { Nombre = "Empresa B", Activo = true };
        context.Empresas.Add(empresa);
        await context.SaveChangesAsync();

        var repository = new EmpresaDA(context, TestDataFactory.Logger<EmpresaDA>());

        var deleted = await repository.DeleteAsync(empresa.Id);

        Assert.True(deleted);
        Assert.Null(await context.Empresas.FirstOrDefaultAsync(e => e.Id == empresa.Id));
    }
}
