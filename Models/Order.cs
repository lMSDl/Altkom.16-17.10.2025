using System.Collections.ObjectModel;

namespace Models
{
    public class Order : Entity
    {
        private string name = string.Empty;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();
    }
}