using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvocaPro.Models;

public class PrioCardModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string CardNumber { get; set; }

    public int? DriverId { get; set; }

    public bool Available { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; }

    [ForeignKey("DriverId")]
    public DriverModel Driver { get; set; }
}