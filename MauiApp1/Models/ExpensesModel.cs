using System.ComponentModel.DataAnnotations;

namespace AdvocaPro.Models;

public class ExpensesModel
{
    [Key]
    public int Id { get; set; }

    public string Description { get; set; }

    public double Amount { get; set; }

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2000, 2100)]
    public int Year { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; }
}