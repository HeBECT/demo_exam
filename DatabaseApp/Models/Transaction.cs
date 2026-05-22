using System;

namespace DatabaseApp.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? DocumentId { get; set; }
        public string DebitAccount { get; set; } = string.Empty;
        public string CreditAccount { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
