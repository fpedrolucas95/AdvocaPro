using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvocaPro.Models;

public class SettlementsModel
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Driver")]
    public int DriverId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Deductions { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Tax { get; set; }

    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; }

    public virtual DriverModel Driver { get; set; }
}
