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

    // ── User-Company relationship via UserDA ────────────────────────────────

    [Fact]
    public async Task UserDA_GetByIdAsync_WhenUserHasCompanies_LoadsCompaniesCollection()
    {
        await using var context = TestDataFactory.CreateContext();
        var company = new Company { Name = "Corp A", Enabled = true };
        context.Companys.Add(company);
        await context.SaveChangesAsync();

        var user = new User
        {
            UserName = "user.company.read",
            Enabled = true,
            Companys = new List<Company> { company }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var userRepo = new UserDA(context, TestDataFactory.Logger<UserDA>());

        var found = await userRepo.GetByIdAsync(user.Id);

        Assert.NotNull(found);
        Assert.Single(found!.Companys);
        Assert.Equal("Corp A", found.Companys[0].Name);
    }

    [Fact]
    public async Task UserDA_UpdateAsync_WhenCompanyAdded_PersistsAssignment()
    {
        await using var context = TestDataFactory.CreateContext();
        var company = new Company { Name = "Corp B", Enabled = true };
        context.Companys.Add(company);
        var user = new User { UserName = "user.assign", Enabled = true, Companys = new List<Company>() };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Simulate what AssignCompanyToUserAsync does
        var userRepo = new UserDA(context, TestDataFactory.Logger<UserDA>());
        var fetched = await userRepo.GetByIdAsync(user.Id);
        fetched!.Companys.Add(company);
        await userRepo.UpdateAsync(fetched);

        var updated = await context.Users.Include(u => u.Companys).FirstAsync(u => u.Id == user.Id);
        Assert.Single(updated.Companys);
        Assert.Equal(company.Id, updated.Companys[0].Id);
    }

    [Fact]
    public async Task UserDA_UpdateAsync_WhenCompanyRemoved_RemovesAssignment()
    {
        await using var context = TestDataFactory.CreateContext();
        var company = new Company { Name = "Corp C", Enabled = true };
        context.Companys.Add(company);
        var user = new User { UserName = "user.remove", Enabled = true, Companys = new List<Company> { company } };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Simulate what RemoveCompanyFromUserAsync does
        var userRepo = new UserDA(context, TestDataFactory.Logger<UserDA>());
        var fetched = await userRepo.GetByIdAsync(user.Id);
        fetched!.Companys.RemoveAll(c => c.Id == company.Id);
        await userRepo.UpdateAsync(fetched);

        var updated = await context.Users.Include(u => u.Companys).FirstAsync(u => u.Id == user.Id);
        Assert.Empty(updated.Companys);
    }
}
