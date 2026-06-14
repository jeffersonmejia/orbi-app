using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.ViewModels;

public class PaymentViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Order is required.")]
    [Display(Name = "Order")]
    public int OrderId { get; set; }

    [Display(Name = "Order Info")]
    public string? OrderInfo { get; set; }

    [Required(ErrorMessage = "Payment method is required.")]
    [Display(Name = "Payment Method")]
    public int PaymentMethodId { get; set; }

    [Display(Name = "Payment Method")]
    public string? PaymentMethodName { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    [DataType(DataType.Currency)]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Display(Name = "Payment Date")]
    public DateTime PaymentDate { get; set; }

    [StringLength(255, ErrorMessage = "Transaction ID cannot exceed 255 characters.")]
    [Display(Name = "Transaction ID")]
    public string? TransactionId { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Pending";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
