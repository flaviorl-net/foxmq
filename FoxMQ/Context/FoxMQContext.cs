using FoxMQ.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FoxMQ.Context
{
    public class FoxMQContext : DbContext
    {
        public FoxMQContext(DbContextOptions<FoxMQContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<MessageQueue> MessageQueue { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var rel in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            {
                rel.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoxMQContext).Assembly);
        }

    }
}
