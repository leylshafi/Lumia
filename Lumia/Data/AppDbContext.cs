using Lumia.Models;
using Lumia.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Lumia.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions options):base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var data in entries)
            {
                switch (data.State)
                {
                    case EntityState.Modified:
                        data.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Added:
                        data.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    default:
                        break;
                }   

            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
