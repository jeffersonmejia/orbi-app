using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.ViewModels;

public class RegisterViewModel
{
    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Phone]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 10)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Street Address")]
    public string? StreetAddress { get; set; }

    [StringLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [StringLength(100)]
    [Display(Name = "State")]
    public string? State { get; set; }

    [StringLength(20)]
    [Display(Name = "Zip Code")]
    public string? ZipCode { get; set; }

    [StringLength(100)]
    [Display(Name = "Country")]
    public string? Country { get; set; }

    [Display(Name = "Store Name")]
    [StringLength(200)]
    public string? StoreName { get; set; }

    [Display(Name = "Store Description")]
    [StringLength(1000)]
    public string? StoreDescription { get; set; }

    [Display(Name = "Store Category")]
    public int? StoreCategoryId { get; set; }
}
