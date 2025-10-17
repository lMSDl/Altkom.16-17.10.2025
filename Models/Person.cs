namespace Models
{
    public class Person : Entity
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int AddressId { get; set; }
        public Address? Address { get; set; }

        public string? OptionalDescription { get; set; } = null;
    }
}
