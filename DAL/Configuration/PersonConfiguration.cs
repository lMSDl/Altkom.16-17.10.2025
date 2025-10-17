using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configuration
{
    internal class PersonConfiguration : EntityConfiguration<Person>
    {
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            base.Configure(builder);

            builder.ToTable(x => x.IsTemporal(x =>
            {
                x.UseHistoryTable("PeopleHistory", "dbo");
                x.HasPeriodStart("ValidFrom");
                x.HasPeriodEnd("ValidTo");
            }));

            builder.Property(x => x.OptionalDescription).IsSparse();
            //istnieje możliwość założenia index na kolumnę sparse
            //builder.HasIndex(x => x.OptionalDescription);
        }
    }
}
