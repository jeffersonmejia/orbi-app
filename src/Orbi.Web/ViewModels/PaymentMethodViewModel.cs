using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.ViewModels;

public class PaymentMethodViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Payment method name is required.")]
    [StringLength(100, ErrorMessage = "Payment method name cannot exceed 100 characters.")]
    [Display(Name = "Payment Method")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
