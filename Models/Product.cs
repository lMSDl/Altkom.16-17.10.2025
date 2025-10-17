using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Models
{
    public class Product : Entity
    {
        private ILazyLoader? _lazyLoader;
        private Order? _order;

        public Product()
        {
        }

        public Product(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        //public int OrderId { get; set; } - Shadow Property generowane automatycznie w EF Core w przypadku relacji
        public Order? Order
        {
            get
            {
                if (_order is null)
                {
                    try
                    {
                        _lazyLoader?.Load(this, ref _order);
                    }
                    catch
                    {
                        _order = null;
                    }
                }
                return _order;
            }
            set => _order = value;
        }

        //odpowiednik IsRowVersion w fluent API
        //[Timestamp]
        //public byte[] Timestamp { get; }
    }
}
