using Accounting.Models;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<Account>? Accounts { get; set; }
    } 
}