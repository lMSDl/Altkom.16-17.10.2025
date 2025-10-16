using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product : Entity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        //public int OrderId { get; set; } - Shadow Property generowane automatycznie w EF Core w przypadku relacji
        public Order? Order { get; set; }

        //odpowiednik IsRowVersion w fluent API
        //[Timestamp]
        //public byte[] Timestamp { get; }
    }
}
