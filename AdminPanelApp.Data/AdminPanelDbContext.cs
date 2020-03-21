namespace AdminPanelApp.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    using Models;
    using Data.Configurations;

    public class AdminPanelDbContext : IdentityDbContext
    {
        public AdminPanelDbContext(DbContextOptions<AdminPanelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }

        public DbSet<Products> Products { get; set; }

        public DbSet<Requisitions> Requisitions { get; set; }

        public DbSet<UserInfo> UsersInfo { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<NextProductsSchedule> NextProductsSchedule { get; set; }

        public DbSet<PreviousSchedule> PreviousSchedules { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new RequisitonsConfig());

            base.OnModelCreating(builder);
        }
    }
}
