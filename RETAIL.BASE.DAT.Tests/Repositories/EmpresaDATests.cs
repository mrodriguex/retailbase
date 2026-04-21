using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class CompanyDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityIsValid_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new CompanyDA(context, TestDataFactory.Logger<CompanyDA>());
        var company = new Company { Name = "Company A", TAXID = "TAXID01", Enabled = true };

        var result = await repository.AddAsync(company);

        Assert.True(result > 0);
        var saved = await context.Companys.FirstOrDefaultAsync(e => e.Id == result);
        Assert.NotNull(saved);
        Assert.Equal("Company A", saved!.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByEnabled_ReturnsOnlyMatchingRows()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Companys.AddRange(
            new Company { Name = "A", Enabled = true },
            new Company { Name = "B", Enabled = false });
        await context.SaveChangesAsync();

        var repository = new CompanyDA(context, TestDataFactory.Logger<CompanyDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(enabled: false))).Data.ToList();

        Assert.Single(result);
        Assert.False(result[0].Enabled);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_RemovesAndReturnsTrue()
    {
        await using var context = TestDataFactory.CreateContext();
        var company = new Company { Name = "Company B", Enabled = true };
        context.Companys.Add(company);
        await context.SaveChangesAsync();

        var repository = new CompanyDA(context, TestDataFactory.Logger<CompanyDA>());

        var deleted = await repository.DeleteAsync(company.Id);

        Assert.True(deleted);
        Assert.Null(await context.Companys.FirstOrDefaultAsync(e => e.Id == company.Id));
    }
}
