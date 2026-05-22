using System;

namespace DatabaseApp.Models
{
    public class CostCalculation
    {
        public int CalcId { get; set; }
        public string ProductName { get; set; } = "";
        public string MaterialName { get; set; } = "";
        public decimal? Quantity { get; set; }
        public string Unit { get; set; } = "";
        public decimal? Price { get; set; }
        public decimal Cost { get; set; }
        public string CalcType { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
