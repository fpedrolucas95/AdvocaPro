namespace AdvocaPro.Models
{
    public class User
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CellPhone { get; set; } = string.Empty;
        public string Registry { get; set; } = string.Empty;
        public int UserType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public string LastEditComputer { get; set; } = string.Empty;
        public DateTime? BirthdayDate { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
