using DAL.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Models;

namespace DAL.Configuration
{
    internal class OrderConfiguration : EntityConfiguration<Order>
    {

        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.OrderDate).IsConcurrencyToken();

            // Computed Column - kolumna wyliczana
            // stored: true - kolumna przechowywana w bazie danych
            builder.Property(x => x.TotalValue).HasComputedColumnSql("[Value] * (1 + [Tax])", stored: true);

            // dane niedeterministyczne (wartość zmienia się przy każdym wywołaniu) nie mogą być kolumnami wyliczanymi składanymi
            builder.Property<DateTime>("CurrentDate").HasComputedColumnSql("GETDATE()");

            builder.Property(x => x.IsExpired).HasComputedColumnSql("CASE WHEN [OrderDate] < GETDATE() THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END");

            builder.Property(x => x.OrderType).HasConversion(
                x => x.ToString(),
                x => Enum.Parse<OrderType>(x)
                );

            //builder.Property(x => x.OrderParameters).HasConversion(new EnumToStringConverter<OrderParameters>());
            builder.Property(x => x.OrderParameters).HasConversion<string>();

            builder.Property(x => x.Name).HasConversion(new ObfuscationConverter());
        }
    }
}
