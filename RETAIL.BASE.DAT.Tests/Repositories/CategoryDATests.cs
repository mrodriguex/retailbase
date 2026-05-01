using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class CategoryDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityIsValid_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());
        var category = new Category { Name = "Category A", Enabled = true };

        var result = await repository.AddAsync(category);

        Assert.True(result > 0);
        var saved = await context.Categories.FirstOrDefaultAsync(c => c.Id == result);
        Assert.NotNull(saved);
        Assert.Equal("Category A", saved!.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByEnabled_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Categories.AddRange(
            new Category { Name = "Active Category", Enabled = true },
            new Category { Name = "Inactive Category", Enabled = false },
            new Category { Name = "Another Active", Enabled = true });
        await context.SaveChangesAsync();

        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(enabled: false))).Data.ToList();

        Assert.Single(result);
        Assert.False(result[0].Enabled);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByName_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Categories.AddRange(
            new Category { Name = "Beverages", Enabled = true },
            new Category { Name = "Snacks", Enabled = true });
        await context.SaveChangesAsync();

        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(name: "Snacks"))).Data.ToList();

        Assert.Single(result);
        Assert.Equal("Snacks", result[0].Name);
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ReturnsCorrectPage()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Categories.AddRange(
            new Category { Name = "Cat 1", Enabled = true },
            new Category { Name = "Cat 2", Enabled = true },
            new Category { Name = "Cat 3", Enabled = true });
        await context.SaveChangesAsync();

        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());

        var pagedResult = await repository.GetAllAsync(TestDataFactory.Paged(pageIndex: 2, pageSize: 2));

        Assert.Single(pagedResult.Data);
        Assert.Equal(3, pagedResult.TotalCount);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        await using var context = TestDataFactory.CreateContext();
        var category = new Category { Name = "Category B", Enabled = true };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());

        var result = await repository.GetByIdAsync(category.Id);

        Assert.NotNull(result);
        Assert.Equal("Category B", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_PersistsChanges()
    {
        await using var context = TestDataFactory.CreateContext();
        var category = new Category { Name = "Old Name", Enabled = true };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());
        category.Name = "New Name";
        var updated = await repository.UpdateAsync(category);

        Assert.True(updated);
        var saved = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == category.Id);
        Assert.Equal("New Name", saved!.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_RemovesAndReturnsTrue()
    {
        await using var context = TestDataFactory.CreateContext();
        var category = new Category { Name = "Category C", Enabled = true };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());

        var deleted = await repository.DeleteAsync(category.Id);

        Assert.True(deleted);
        Assert.Null(await context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new CategoryDA(context, TestDataFactory.Logger<CategoryDA>());

        var deleted = await repository.DeleteAsync(999);

        Assert.False(deleted);
    }
}
