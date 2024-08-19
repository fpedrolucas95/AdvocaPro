using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvocaPro.Models;

public enum EntityType
{
    Driver = 0,
    Owner = 1
}

public class BankInformationModel
{
    [Key]
    public int Id { get; set; }

    public int EntityId { get; set; }

    public EntityType EntityType { get; set; }

    [StringLength(255)]
    public string BankName { get; set; }

    [StringLength(50)]
    public string AccountNumber { get; set; }

    [StringLength(34)]
    public string Iban { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string UpdatedBy { get; set; }

    [ForeignKey("EntityId")]
    public virtual DriverModel Driver { get; set; }

    [ForeignKey("EntityId")]
    public virtual VehicleModel Owner { get; set; }
}
