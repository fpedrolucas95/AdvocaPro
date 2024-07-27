namespace AdvocaPro.Models
{
    public class Settings
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public byte[]? Logo { get; set; } = Array.Empty<byte>();
        public string Website { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Instagram { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public int Theme { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
