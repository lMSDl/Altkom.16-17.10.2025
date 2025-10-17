using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DAL.Conventions
{
    internal class StringObfuscationConvention : IModelFinalizingConvention
    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var property in modelBuilder.Metadata.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(string))
                         .Where(x => x.PropertyInfo?.CanWrite ?? false))
            {
                property.SetValueConverter(new Converters.ObfuscationConverter());
            }
        }
    }
}
