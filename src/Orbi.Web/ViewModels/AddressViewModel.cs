using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.ViewModels;

public class AddressViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer is required.")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Display(Name = "Customer Name")]
    public string? CustomerName { get; set; }

    [Required(ErrorMessage = "Street is required.")]
    [StringLength(255, ErrorMessage = "Street cannot exceed 255 characters.")]
    [Display(Name = "Street")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required.")]
    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
    [Display(Name = "State")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Zip code is required.")]
    [StringLength(20, ErrorMessage = "Zip code cannot exceed 20 characters.")]
    [Display(Name = "Zip Code")]
    public string ZipCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required.")]
    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
    [Display(Name = "Country")]
    public string Country { get; set; } = string.Empty;

    [Display(Name = "Latitude")]
    public double? Latitude { get; set; }

    [Display(Name = "Longitude")]
    public double? Longitude { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
