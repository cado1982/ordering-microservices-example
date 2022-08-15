using Microsoft.EntityFrameworkCore;
using Ordering.Models;

namespace Ordering.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<Order>? Orders { get; set; }
        public DbSet<Account>? Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Account>()
                .HasMany(a => a.Orders)
                .WithOne(o => o.Account!)
                .HasForeignKey(a => a.AccountId);

            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.Account)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountId);

            modelBuilder
                .Entity<Order>()
                .Property(o => o.Cost)
                .HasColumnType("money");

        }
    }
}