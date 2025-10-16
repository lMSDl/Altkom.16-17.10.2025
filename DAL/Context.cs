using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    public class Context : DbContext
    {
        public Context() { }
        public Context(DbContextOptions<Context> options) : base(options) { }

        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            base.OnModelCreating(modelBuilder);
        }

        public bool RandomFail { get; set; } 

        public override int SaveChanges()
        {
            /*ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Where(x => x.Entity is Entity)
                .ToList()
                .ForEach(e =>
                {
                    e.Property(nameof(Entity.CreatedDate)).IsModified = false;
                });*/

            if(RandomFail && Random.Shared.Next(1, 25) == 1)
            {
                throw new DbUpdateException("Losowy błąd podczas zapisu do bazy");
            }

            return base.SaveChanges();
        }
    }
}
