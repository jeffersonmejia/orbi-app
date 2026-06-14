using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.ViewModels;

public class DeliveryDriverViewModel
{
    public int Id { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => $"{FirstName} {LastName}";

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Current Latitude")]
    public double CurrentLatitude { get; set; }

    [Display(Name = "Current Longitude")]
    public double CurrentLongitude { get; set; }

    [Display(Name = "Last Location Update")]
    public DateTime LastLocationUpdate { get; set; }

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; } = true;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
