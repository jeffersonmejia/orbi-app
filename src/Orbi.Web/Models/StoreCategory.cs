using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.Models;

public class StoreCategory : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public ICollection<Store> Stores { get; set; } = new List<Store>();
}
