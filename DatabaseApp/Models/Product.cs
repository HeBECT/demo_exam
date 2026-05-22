using System;

namespace DatabaseApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
