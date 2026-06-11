using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.Models;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
