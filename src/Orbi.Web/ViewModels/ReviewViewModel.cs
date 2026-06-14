using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.ViewModels;

public class ReviewViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer is required.")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Display(Name = "Customer Name")]
    public string? CustomerName { get; set; }

    [Required(ErrorMessage = "Store is required.")]
    [Display(Name = "Store")]
    public int StoreId { get; set; }

    [Display(Name = "Store Name")]
    public string? StoreName { get; set; }

    [Required(ErrorMessage = "Rating is required.")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    [Display(Name = "Rating")]
    public int Rating { get; set; }

    [StringLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters.")]
    [Display(Name = "Comment")]
    public string? Comment { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
