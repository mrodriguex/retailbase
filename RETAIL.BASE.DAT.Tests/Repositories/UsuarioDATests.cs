using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class UserDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityHasExistingRelations_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var company = new Company { Name = "Company A", Enabled = true };
        var role = new Role { Name = "Role A", Enabled = true };
        context.Companys.Add(company);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var user = new User
        {
            UserName = "user.test",
            FirstName = "Test",
            Enabled = true,
            Companys = new List<Company> { company },
            Roles = new List<Role> { role }
        };

        var repository = new UserDA(context, TestDataFactory.Logger<UserDA>());

        var id = await repository.AddAsync(user);

        Assert.True(id > 0);
        var saved = await context.Users
            .Include(u => u.Companys)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
        Assert.NotNull(saved);
        Assert.Single(saved!.Companys);
        Assert.Single(saved.Roles);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByStatusAndUsername_ReturnsMatches()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Users.AddRange(
            new User { UserName = "admin.one", Enabled = true },
            new User { UserName = "guest.one", Enabled = false },
            new User { UserName = "admin.two", Enabled = true });
        await context.SaveChangesAsync();

        var repository = new UserDA(context, TestDataFactory.Logger<UserDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(enabled: true, name: "admin", pageIndex: 1, pageSize: 10))).Data.ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, u => Assert.True(u.Enabled));
    }

    [Fact]
    public async Task UpdateAsync_ThenGetByIdAsync_PersistsNameAndFirstNameIndependently()
    {
        const string databaseName = "User_Update_GetById_PersistsNames";

        await using (var seedContext = TestDataFactory.CreateContext(databaseName))
        {
            seedContext.Users.Add(new User
            {
                Id = 1,
                UserName = "user.test",
                Name = "Name inicial",
                FirstName = "login.inicial",
                Email = "user@test.com",
                Password = "secret",
                Enabled = true
            });
            await seedContext.SaveChangesAsync();
        }

        await using (var updateContext = TestDataFactory.CreateContext(databaseName))
        {
            var repository = new UserDA(updateContext, TestDataFactory.Logger<UserDA>());
            var updated = new User
            {
                Id = 1,
                UserName = "user.test",
                Name = "Name actualizado",
                FirstName = "login.actualizado",
                Email = "user@test.com",
                Password = "secret",
                Enabled = true,
                Companys = new List<Company>(),
                Roles = new List<Role>()
            };

            var result = await repository.UpdateAsync(updated);

            Assert.True(result);
        }

        await using (var readContext = TestDataFactory.CreateContext(databaseName))
        {
            var repository = new UserDA(readContext, TestDataFactory.Logger<UserDA>());

            var user = await repository.GetByIdAsync(1);

            Assert.NotNull(user);
            Assert.Equal("Name actualizado", user!.Name);
            Assert.Equal("login.actualizado", user.FirstName);
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new UserDA(context, TestDataFactory.Logger<UserDA>());

        var deleted = await repository.DeleteAsync(404);

        Assert.False(deleted);
    }
}
