using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.Models;

public class PaymentMethod : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
