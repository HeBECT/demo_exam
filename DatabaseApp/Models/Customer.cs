namespace DatabaseApp.Models
{
    public class Customer
    {
        public string CustomerId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Inn { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public bool IsSalesman { get; set; }
        public bool IsBuyer { get; set; }
    }
}
