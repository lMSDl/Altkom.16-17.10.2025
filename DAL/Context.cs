using DAL.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Models;

namespace DAL
{
    public class Context : DbContext
    {
        public Context() { }
        public Context(DbContextOptions<Context> options) : base(options) { }

        public static Func<Context, int, Product> GetProductsByDateTime { get; } =
            EF.CompileQuery((Context context, int days) =>
            context.Set<Product>()
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Products)
                    .Where(x => x.Id % 2 == 0)
                    .Where(x => x.Order.Id % 2 == 0)
                    .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(days))
                    .OrderByDescending(x => x.Order.OrderDate)
                    .First()
            );


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

            //możemy wpływać na sposób dostępu do właściwości w modelu, np. preferując dostęp przez właściwości zamiast pól
            modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferProperty);
            //domyślne ustawienie to PropertyAccessMode.PreferFieldDuringConstruction, czyli dostęp przez pola podczas tworzenia obiektu, a potem przez właściwości
            //modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);


            modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetProperties())
                .Where(x => x.ClrType == typeof(int))
                .Where(x => x.Name == "Key")
                .ToList()
                .ForEach(x =>
                {
                    x.IsNullable = false;
                    ((IMutableEntityType)x.DeclaringType).SetPrimaryKey(x);
                });


        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            //configurationBuilder.Properties<DateTime>().HavePrecision(5);

            configurationBuilder.Conventions.Add(_ => new DateTimePrecisionConvention());
            configurationBuilder.Conventions.Add(_ => new PluralizeTableNameConvention());
            configurationBuilder.Conventions.Add(_ => new StringObfuscationConvention());

            //configurationBuilder.Conventions.Remove(typeof(KeyDiscoveryConvention));
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
