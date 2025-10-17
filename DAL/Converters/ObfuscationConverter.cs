using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Converters
{
    internal class ObfuscationConverter : ValueConverter<string, string>
    {
        public ObfuscationConverter() :
            base(x => ToDatabase(x), x => FromDatabase(x))
        {
        }

        private static string ToDatabase(string @string)
        {
            if (@string == null)
                return null;
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(@string));
        }

        private static string FromDatabase(string dbString)
        {
            if (dbString == null)
                return null;
            var bytes = Convert.FromBase64String(dbString);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
