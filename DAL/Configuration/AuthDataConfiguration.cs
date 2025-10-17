using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configuration
{
    internal class AuthDataConfiguration : IEntityTypeConfiguration<AuthData>
    {
        public void Configure(EntityTypeBuilder<AuthData> builder)
        {
            builder.ToTable("Logins");
            //builder.HasKey(x => x.Key);
        }
    }
}
