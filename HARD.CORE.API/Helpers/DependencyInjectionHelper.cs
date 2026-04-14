using HARD.CORE.DAT;
using HARD.CORE.DAT.Interfaces;
using HARD.CORE.DAT.Repositories;
using HARD.CORE.NEG.Helpers;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.NEG.Services;
using HARD.CORE.OBJ;
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
        services.AddScoped<IRepositoryBase<Cliente, BaseFilter, int>, ClienteDA>();
        services.AddScoped<IRepositoryBase<Empresa, BaseFilter, int>, EmpresaDA>();
        services.AddScoped<IRepositoryBase<Menu, BaseFilter, int>, MenuDA>();
        services.AddScoped<IRepositoryBase<Perfil, BaseFilter, int>, PerfilDA>();
        services.AddScoped<IRepositoryBase<Usuario, BaseFilter, int>, UsuarioDA>();

        // Register services B

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IPerfilService, PerfilService>();
        services.AddScoped<IUsuarioService, UsuarioService>();

        services.AddScoped<ConfigService>();
        services.AddScoped<ICryptographerB, CryptographerSHA512B>();
        services.AddScoped<ICryptographerService, CryptographerService>();

        // Register DbContext
        services.AddDbContext<HardCoreDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("HARD.CORE.DAT")
            ));

        return services;
    }
}