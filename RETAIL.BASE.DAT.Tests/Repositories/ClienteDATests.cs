using RETAIL.BASE.DAT.Tests.Helpers;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

namespace RETAIL.BASE.DAT.Tests.Repositories;

public class CustomerDATests
{
    [Fact]
    public async Task AddAsync_WhenEntityIsValid_PersistsAndReturnsId()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new CustomerDA(context, TestDataFactory.Logger<CustomerDA>());
        var customer = new Customer { Name = "Customer A", TAXID = "XAXX010101000", Enabled = true };

        var result = await repository.AddAsync(customer);

        Assert.True(result > 0);
        var saved = await context.Customers.FirstOrDefaultAsync(c => c.Id == result);
        Assert.NotNull(saved);
        Assert.Equal("Customer A", saved!.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenFilteringByEnabled_ReturnsPagedData()
    {
        await using var context = TestDataFactory.CreateContext();
        context.Customers.AddRange(
            new Customer { Name = "A", Enabled = true },
            new Customer { Name = "B", Enabled = false },
            new Customer { Name = "C", Enabled = true });
        await context.SaveChangesAsync();

        var repository = new CustomerDA(context, TestDataFactory.Logger<CustomerDA>());

        var result = (await repository.GetAllAsync(TestDataFactory.Paged(enabled: true, pageIndex: 1, pageSize: 1))).Data.ToList();

        Assert.Single(result);
        Assert.True(result[0].Enabled);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDataFactory.CreateContext();
        var repository = new CustomerDA(context, TestDataFactory.Logger<CustomerDA>());

        var deleted = await repository.DeleteAsync(999);

        Assert.False(deleted);
    }
}
