using System;

namespace DatabaseApp.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public int? CounterpartyId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
