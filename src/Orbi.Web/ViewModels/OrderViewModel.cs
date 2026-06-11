using System.ComponentModel.DataAnnotations;

namespace Orbi.Web.ViewModels;

public class OrderViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer is required.")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Store is required.")]
    [Display(Name = "Store")]
    public int StoreId { get; set; }

    [Display(Name = "Delivery Driver")]
    public int? DeliveryDriverId { get; set; }

    [Required(ErrorMessage = "Order status is required.")]
    [Display(Name = "Order Status")]
    public int OrderStatusId { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [Display(Name = "Delivery Address")]
    public int AddressId { get; set; }

    [Display(Name = "Total Amount")]
    [DataType(DataType.Currency)]
    public decimal TotalAmount { get; set; }

    [Display(Name = "Order Date")]
    public DateTime OrderDate { get; set; }

    [Display(Name = "Delivery Date")]
    public DateTime? DeliveryDate { get; set; }

    [Display(Name = "Customer Name")]
    public string? CustomerName { get; set; }

    [Display(Name = "Store Name")]
    public string? StoreName { get; set; }

    [Display(Name = "Driver Name")]
    public string? DriverName { get; set; }

    [Display(Name = "Order Status")]
    public string? OrderStatusName { get; set; }

    [Display(Name = "Delivery Address")]
    public string? AddressLine { get; set; }

    public bool IsActive { get; set; } = true;

    public List<OrderItemViewModel> Items { get; set; } = new();
}

public class OrderItemViewModel
{
    public int ProductId { get; set; }

    [Display(Name = "Product")]
    public string? ProductName { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Subtotal")]
    [DataType(DataType.Currency)]
    public decimal Subtotal { get; set; }
}
