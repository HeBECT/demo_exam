using System;

namespace DatabaseApp.Models
{
    public class CustomerOrder
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public string Executor { get; set; } = "";
        public string CustomerId { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
