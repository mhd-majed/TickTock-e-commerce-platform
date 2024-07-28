
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using e_commerce_platform.Models;

namespace e_commerce_platform.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);
            builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Address>().HasQueryFilter(p => !p.IsDeleted);
        }
        public DbSet<e_commerce_platform.Models.Address> Address { get; set; } = default!;
        public DbSet<e_commerce_platform.Models.Testimonial> Testimonial { get; set; } = default!;
        public DbSet<e_commerce_platform.Models.Category> Category { get; set; } = default!;
        public DbSet<e_commerce_platform.Models.Product> Product { get; set; } = default!;
    }
}