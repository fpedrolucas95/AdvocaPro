namespace AdvocaPro.Models
{
    public class Finance
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool Received { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientCellPhone { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public decimal TotalAgreed { get; set; }
        public int Installments { get; set; }
        public decimal InstallmentValue { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal ReceivedAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
