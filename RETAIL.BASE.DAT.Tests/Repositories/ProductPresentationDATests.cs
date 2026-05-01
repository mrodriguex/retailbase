using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class ProductPresentationDATests
{
    private static async Task<Product> SeedProductAsync(TestRETAIL_BASEDbContext context)
    {
        var product = new Product { Name = "Parent Product", BrandId = 1, CategoryId = 1, Enabled = true };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    [Fact]
    public async Task AddAsync_WhenEntityIsValid_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var product = await SeedProductAsync(context);
        var repository = new ProductPresentationDA(context, TestDataFactory.Logger<ProductPresentationDA>());
        var presentation = new ProductPresentation
        {
            Name = "Lata 355ml",
            ProductId = product.Id,
            Barcode = "001122334455",
            SizeLabel = "355ml",
            Unit = "ml",
            Presentation = "Lata",
            Enabled = true
        };

        var result = await repository.AddAsync(presentation);

        Assert.True(result > 0);
        var saved = await context.ProductPresentations.FirstOrDefaultAsync(p => p.Id == result);
        Assert.NotNull(saved);
        Assert.Equal("Lata 355ml", saved!.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByEnabled_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        var product = await SeedProductAsync(context);
        context.ProductPresentations.AddRange(
            new ProductPresentation { Name = "PP Active", ProductId = product.Id, Barcode = "A1", SizeLabel = "1L", Unit = "L", Presentation = "Botella", Enabled = true },
            new ProductPresentation { Name = "PP Inactive", ProductId = product.Id, Barcode = "A2", SizeLabel = "2L", Unit = "L", Presentation = "Botella", Enabled = false });
        await context.SaveChangesAsync();

        var repository = new ProductPresentationDA(context, TestDataFactory.Logger<ProductPresentationDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(enabled: true))).Data.ToList();

        Assert.Single(result);
        Assert.True(result[0].Enabled);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByIdMaster_ReturnsOnlyMatchingProduct()
    {
        await using var context = TestDataFactory.CreateContext();
        var product1 = await SeedProductAsync(context);
        var product2 = new Product { Name = "Product 2", BrandId = 1, CategoryId = 1, Enabled = true };
        context.Products.Add(product2);
        await context.SaveChangesAsync();

        context.ProductPresentations.AddRange(
            new ProductPresentation { Name = "PP1", ProductId = product1.Id, Barcode = "B1", SizeLabel = "1L", Unit = "L", Presentation = "Caja", Enabled = true },
            new ProductPresentation { Name = "PP2", ProductId = product2.Id, Barcode = "B2", SizeLabel = "2L", Unit = "L", Presentation = "Caja", Enabled = true });
        await context.SaveChangesAsync();

        var repository = new ProductPresentationDA(context, TestDataFactory.Logger<ProductPresentationDA>());

        var filter = new BaseFilter { IdMaster = product1.Id, PageIndex = 1, PageSize = 50 };
        var result = (await repository.GetAllAsync(filter)).Data.ToList();

        Assert.Single(result);
        Assert.Equal(product1.Id, result[0].ProductId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        await using var context = TestDataFactory.CreateContext();
        var product = await SeedProductAsync(context);
        var presentation = new ProductPresentation
        {
            Name = "PP Get",
            ProductId = product.Id,
            Barcode = "C1",
            SizeLabel = "500ml",
            Unit = "ml",
            Presentation = "Botella",
            Enabled = true
        };
        context.ProductPresentations.Add(presentation);
        await context.SaveChangesAsync();

        var repository = new ProductPresentationDA(context, TestDataFactory.Logger<ProductPresentationDA>());

        var result = await repository.GetByIdAsync(presentation.Id);

        Assert.NotNull(result);
        Assert.Equal("PP Get", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new ProductPresentationDA(context, TestDataFactory.Logger<ProductPresentationDA>());

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_PersistsChanges()
    {
        await using var context = TestDataFactory.CreateContext();
        var product = await SeedProductAsync(context);
        var presentation = new ProductPresentation
        {
            Name = "Old Name",
            ProductId = product.Id,
            Barcode = "D1",
            SizeLabel = "1L",
            Unit = "L",
            Presentation = "Botella",
            Enabled = true
        };
        context.ProductPresentations.Add(presentation);
        await context.SaveChangesAsync();

        var repository = new ProductPresentationDA(context, TestDataFactory.Logger<ProductPresentationDA>());
        presentation.Name = "New Name";
        var updated = await repository.UpdateAsync(presentation);

        Assert.True(updated);
        var saved = await context.ProductPresentations.AsNoTracking().FirstOrDefaultAsync(p => p.Id == presentation.Id);
        Assert.Equal("New Name", saved!.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_RemovesAndReturnsTrue()
    {
        await using var context = TestDataFactory.CreateContext();
        var product = await SeedProductAsync(context);
        var presentation = new ProductPresentation
        {
            Name = "PP Delete",
            ProductId = product.Id,
            Barcode = "E1",
            SizeLabel = "2L",
            Unit = "L",
            Presentation = "Garrafa",
            Enabled = true
        };
        context.ProductPresentations.Add(presentation);
        await context.SaveChangesAsync();

        var repository = new ProductPresentationDA(context, TestDataFactory.Logger<ProductPresentationDA>());

        var deleted = await repository.DeleteAsync(presentation.Id);

        Assert.True(deleted);
        Assert.Null(await context.ProductPresentations.FirstOrDefaultAsync(p => p.Id == presentation.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new ProductPresentationDA(context, TestDataFactory.Logger<ProductPresentationDA>());

        var deleted = await repository.DeleteAsync(999);

        Assert.False(deleted);
    }
}
