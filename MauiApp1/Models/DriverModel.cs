using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvocaPro.Models;

public class DriverModel
{
    [Key]
    public int Id { get; set; }
    public string UberUid { get; set; }
    public string BoltUid { get; set; }

    [Required]
    public string Name { get; set; }

    public double? ContractPercentage { get; set; }

    [Column(TypeName = "Date")]
    public DateTime? VacationStart { get; set; }

    public int? VacationDays { get; set; }

    public string Remarks { get; set; }

    public string PrioCard { get; set; }

    public string VehicleRegistration { get; set; }

    [Column(TypeName = "Date")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "Date")]
    public DateTime? EndDate { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string Nif { get; set; }

    public string DrivingLicense { get; set; }

    public string BankName { get; set; }

    public string AccountNumber { get; set; }

    public string Iban { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; }
}
