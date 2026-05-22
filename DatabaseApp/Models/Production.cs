using System;

namespace DatabaseApp.Models
{
    public class Production
    {
        public int ProductionId { get; set; }
        public string ProductionNumber { get; set; } = "";
        public DateTime ProductionDate { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
