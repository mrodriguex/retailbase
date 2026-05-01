using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class BrandDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityIsValid_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new BrandDA(context, TestDataFactory.Logger<BrandDA>());
        var brand = new Brand { Name = "Brand A", Enabled = true };

        var result = await repository.AddAsync(brand);

        Assert.True(result > 0);
        var saved = await context.Brands.FirstOrDefaultAsync(b => b.Id == result);
        Assert.NotNull(saved);
        Assert.Equal("Brand A", saved!.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByEnabled_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Brands.AddRange(
            new Brand { Name = "Active Brand", Enabled = true },
            new Brand { Name = "Inactive Brand", Enabled = false },
            new Brand { Name = "Another Active", Enabled = true });
        await context.SaveChangesAsync();

        var repository = new BrandDA(context, TestDataFactory.Logger<BrandDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(enabled: true))).Data.ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, b => Assert.True(b.Enabled));
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByName_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Brands.AddRange(
            new Brand { Name = "Pepsi", Enabled = true },
            new Brand { Name = "Coca-Cola", Enabled = true });
        await context.SaveChangesAsync();

        var repository = new BrandDA(context, TestDataFactory.Logger<BrandDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(name: "Pepsi"))).Data.ToList();

        Assert.Single(result);
        Assert.Equal("Pepsi", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        await using var context = TestDataFactory.CreateContext();
        var brand = new Brand { Name = "Brand B", Enabled = true };
        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandDA(context, TestDataFactory.Logger<BrandDA>());

        var result = await repository.GetByIdAsync(brand.Id);

        Assert.NotNull(result);
        Assert.Equal("Brand B", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new BrandDA(context, TestDataFactory.Logger<BrandDA>());

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_PersistsChanges()
    {
        await using var context = TestDataFactory.CreateContext();
        var brand = new Brand { Name = "Old Name", Enabled = true };
        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandDA(context, TestDataFactory.Logger<BrandDA>());
        brand.Name = "New Name";
        var updated = await repository.UpdateAsync(brand);

        Assert.True(updated);
        var saved = await context.Brands.AsNoTracking().FirstOrDefaultAsync(b => b.Id == brand.Id);
        Assert.Equal("New Name", saved!.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_RemovesAndReturnsTrue()
    {
        await using var context = TestDataFactory.CreateContext();
        var brand = new Brand { Name = "Brand C", Enabled = true };
        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        var repository = new BrandDA(context, TestDataFactory.Logger<BrandDA>());

        var deleted = await repository.DeleteAsync(brand.Id);

        Assert.True(deleted);
        Assert.Null(await context.Brands.FirstOrDefaultAsync(b => b.Id == brand.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new BrandDA(context, TestDataFactory.Logger<BrandDA>());

        var deleted = await repository.DeleteAsync(999);

        Assert.False(deleted);
    }
}
