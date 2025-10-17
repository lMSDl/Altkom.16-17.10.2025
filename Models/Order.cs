using NetTopologySuite.Geometries;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Order : Entity
    {
        //private string name = string.Empty;
        private string _name = string.Empty;
        //private string m_name = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        //odpowiednik IsConcurrencyToken w fluent API
        //[ConcurrencyCheck]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public virtual ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public float Value { get; set; }
        public float Tax { get; set; }

        //public float TotalValue => Value * (1 + Tax);
        public float TotalValue { get; }

        public bool IsExpired { get; }

        public OrderType OrderType { get; set; } = OrderType.Standard;
        public OrderParameters OrderParameters { get; set; } = OrderParameters.None;


        public Point? DeliveryPoint { get; set; }
    }
}