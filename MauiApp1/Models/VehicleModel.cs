using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvocaPro.Models;

public class VehicleModel
{
    [Key]
    public string Registration { get; set; }

    public int? OwnerId { get; set; }

    public double? Insurance { get; set; }

    public string Remarks { get; set; }

    public bool Available { get; set; } = true;

    public string OwnerBankName { get; set; }

    public string OwnerAccountNumber { get; set; }

    public string OwnerIban { get; set; }

    public string OwnerPhone { get; set; }

    public string OwnerEmail { get; set; }

    public string OwnerNif { get; set; }

    public string OwnerDrivingLicense { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; }

    [ForeignKey("OwnerId")]
    public DriverModel Owner { get; set; }
}