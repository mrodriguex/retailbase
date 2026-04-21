using RETAIL.BASE.DAT;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.DAT.Repositories;
using RETAIL.BASE.NEG.Helpers;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.NEG.Services;
using RETAIL.BASE.OBJ;
using Microsoft.EntityFrameworkCore;

public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the DI container.
    /// </summary>
    /// <param name="services">
    /// The service collection to add services to.
    /// </param>
    /// <param name="configuration">
    /// The configuration to use.
    /// </param>
    /// <returns>
    /// The service collection.
    /// </returns>      
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        services.AddSingleton<IConfiguration>(configuration);


        // Register services DA
        services.AddScoped<IRepositoryBase<Customer, BaseFilter, int>, CustomerDA>();
        services.AddScoped<IRepositoryBase<Company, BaseFilter, int>, CompanyDA>();
        services.AddScoped<IRepositoryBase<MenuItem, BaseFilter, int>, MenuItemDA>();
        services.AddScoped<IRepositoryBase<Role, BaseFilter, int>, RoleDA>();
        services.AddScoped<IRepositoryBase<User, BaseFilter, int>, UserDA>();
        services.AddScoped<IRepositoryBase<Brand, BaseFilter, int>, BrandDA>();
        services.AddScoped<IRepositoryBase<Category, BaseFilter, int>, CategoryDA>();
        services.AddScoped<IRepositoryBase<Product, BaseFilter, int>, ProductDA>();
        services.AddScoped<IRepositoryBase<ProductPresentation, BaseFilter, int>, ProductPresentationDA>();

        // Register services B

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductPresentationService, ProductPresentationService>();

        services.AddScoped<ConfigService>();
        services.AddScoped<ICryptographerB, CryptographerSHA512B>();
        services.AddScoped<ICryptographerService, CryptographerService>();

        services.AddScoped<HubCommunicationService>();

        // Register DbContext
        services.AddDbContext<RETAIL_BASEDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("SqlConn_RETAIL_BASE"),
                b => b.MigrationsAssembly("RETAIL.BASE.DAT")
            ));



        return services;
    }
}