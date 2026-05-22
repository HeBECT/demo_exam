using System;

namespace DatabaseApp.Models
{
    public class Specification
    {
        public int SpecId { get; set; }
        public string SpecName { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
