using System;

namespace DatabaseApp.Models
{
    public class Counterparty
    {
        public int CounterpartyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string Kpp { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string CounterpartyType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
