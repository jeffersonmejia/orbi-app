using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.Models;

public class DeliveryDriver : BaseEntity
{
    public string? UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    public double CurrentLatitude { get; set; }

    public double CurrentLongitude { get; set; }

    public DateTime LastLocationUpdate { get; set; }

    [Required]
    public bool IsAvailable { get; set; } = true;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
