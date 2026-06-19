using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }
}
