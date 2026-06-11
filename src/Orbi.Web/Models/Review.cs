using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbi.Web.Models;

public class Review : BaseEntity
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int StoreId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    [ForeignKey(nameof(StoreId))]
    public Store Store { get; set; } = null!;
}
