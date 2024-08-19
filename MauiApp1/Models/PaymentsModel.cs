using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvocaPro.Models;

public class PaymentsModel
{
    [Key]
    public int Id { get; set; }

    public int? DriverId { get; set; }

    public double AmountPaid { get; set; }

    [Column(TypeName = "Date")]
    public DateTime PaymentDate { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; }

    [ForeignKey("DriverId")]
    public DriverModel Driver { get; set; }
}