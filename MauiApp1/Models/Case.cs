namespace AdvocaPro.Models
{
    public class Case
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Court { get; set; } = string.Empty;
        public string Opponent { get; set; } = string.Empty;
        public string CaseDetails { get; set; } = string.Empty;
        public string CaseObservation { get; set; } = string.Empty;
        public bool CaseCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public Client Client { get; set; } = new Client();
    }
}
