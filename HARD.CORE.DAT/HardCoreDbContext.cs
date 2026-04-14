using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HARD.CORE.OBJ;
using System.Data;
using System.Collections.Generic;

namespace HARD.CORE.DAT
{
    public class HardCoreDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public HardCoreDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("SqlConn_HARDCORE");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // DbSets for entities
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Producto> Productos { get; set; }
        // public DbSet<UsuarioPerfil> UsuarioPerfiles { get; set; }
        // public DbSet<PerfilMenu> PerfilMenus { get; set; }
    }

}