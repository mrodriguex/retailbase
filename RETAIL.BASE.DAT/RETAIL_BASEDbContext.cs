using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RETAIL.BASE.OBJ;
using System.Data;
using System.Collections.Generic;

namespace RETAIL.BASE.DAT
{
    public class RETAIL_BASEDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public RETAIL_BASEDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("SqlConn_RETAIL_BASE");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // DbSets for entities
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPresentation> ProductPresentations { get; set; }
        // public DbSet<UserRole> UserRoles { get; set; }
        // public DbSet<RoleMenuItem> RoleMenuItems { get; set; }
    }

}