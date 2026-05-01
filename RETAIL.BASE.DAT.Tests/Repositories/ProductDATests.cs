using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class ProductDATests
{
    private static async Task<(Brand brand, Category category)> SeedParentsAsync(TestRETAIL_BASEDbContext context)
    {
        var brand = new Brand { Name = "Test Brand", Enabled = true };
        var category = new Category { Name = "Test Category", Enabled = true };
        context.Brands.Add(brand);
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return (brand, category);
    }

    [Fact]
    public async Task AddAsync_WhenEntityIsValid_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var (brand, category) = await SeedParentsAsync(context);
        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());
        var product = new Product { Name = "Product A", BrandId = brand.Id, CategoryId = category.Id, Enabled = true };

        var result = await repository.AddAsync(product);

        Assert.True(result > 0);
        var saved = await context.Products.FirstOrDefaultAsync(p => p.Id == result);
        Assert.NotNull(saved);
        Assert.Equal("Product A", saved!.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByEnabled_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        var (brand, category) = await SeedParentsAsync(context);
        context.Products.AddRange(
            new Product { Name = "Active Product", BrandId = brand.Id, CategoryId = category.Id, Enabled = true },
            new Product { Name = "Inactive Product", BrandId = brand.Id, CategoryId = category.Id, Enabled = false },
            new Product { Name = "Another Active", BrandId = brand.Id, CategoryId = category.Id, Enabled = true });
        await context.SaveChangesAsync();

        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(enabled: true))).Data.ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.True(p.Enabled));
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByName_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        var (brand, category) = await SeedParentsAsync(context);
        context.Products.AddRange(
            new Product { Name = "Pepsi 355ml", BrandId = brand.Id, CategoryId = category.Id, Enabled = true },
            new Product { Name = "Coca-Cola 355ml", BrandId = brand.Id, CategoryId = category.Id, Enabled = true });
        await context.SaveChangesAsync();

        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(name: "Pepsi"))).Data.ToList();

        Assert.Single(result);
        Assert.Equal("Pepsi 355ml", result[0].Name);
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ReturnsCorrectTotalCount()
    {
        await using var context = TestDataFactory.CreateContext();
        var (brand, category) = await SeedParentsAsync(context);
        context.Products.AddRange(
            new Product { Name = "P1", BrandId = brand.Id, CategoryId = category.Id, Enabled = true },
            new Product { Name = "P2", BrandId = brand.Id, CategoryId = category.Id, Enabled = true },
            new Product { Name = "P3", BrandId = brand.Id, CategoryId = category.Id, Enabled = true });
        await context.SaveChangesAsync();

        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());

        var pagedResult = await repository.GetAllAsync(TestDataFactory.Paged(pageIndex: 1, pageSize: 2));

        Assert.Equal(2, pagedResult.Data.Count());
        Assert.Equal(3, pagedResult.TotalCount);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        await using var context = TestDataFactory.CreateContext();
        var (brand, category) = await SeedParentsAsync(context);
        var product = new Product { Name = "Product B", BrandId = brand.Id, CategoryId = category.Id, Enabled = true };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());

        var result = await repository.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Product B", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_PersistsChanges()
    {
        await using var context = TestDataFactory.CreateContext();
        var (brand, category) = await SeedParentsAsync(context);
        var product = new Product { Name = "Old Name", BrandId = brand.Id, CategoryId = category.Id, Enabled = true };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());
        product.Name = "New Name";
        var updated = await repository.UpdateAsync(product);

        Assert.True(updated);
        var saved = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.Equal("New Name", saved!.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_RemovesAndReturnsTrue()
    {
        await using var context = TestDataFactory.CreateContext();
        var (brand, category) = await SeedParentsAsync(context);
        var product = new Product { Name = "Product C", BrandId = brand.Id, CategoryId = category.Id, Enabled = true };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());

        var deleted = await repository.DeleteAsync(product.Id);

        Assert.True(deleted);
        Assert.Null(await context.Products.FirstOrDefaultAsync(p => p.Id == product.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new ProductDA(context, TestDataFactory.Logger<ProductDA>());

        var deleted = await repository.DeleteAsync(999);

        Assert.False(deleted);
    }
}
