namespace AdvocaPro.Models
{
    public class Deadline
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationDays { get; set; }
        public bool IsBusinessDays { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public Client? Client { get; set; }
    }

    public class ClientPickerItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}