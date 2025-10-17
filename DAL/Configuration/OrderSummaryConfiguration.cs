using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configuration
{
    internal class OrderSummaryConfiguration : IEntityTypeConfiguration<OrderSummary>
    {
        public void Configure(EntityTypeBuilder<OrderSummary> builder)
        {
            builder.ToView("View_OrderSummary");
        }
    }
}
