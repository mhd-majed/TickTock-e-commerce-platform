
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

            // Apply a global filter to exclude soft deleted users
            builder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);
        }
        public DbSet<e_commerce_platform.Models.Address> Address { get; set; } = default!;
        public DbSet<e_commerce_platform.Models.Testimonial> Testimonial { get; set; } = default!;
    }
}