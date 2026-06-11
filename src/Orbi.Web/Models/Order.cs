using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbi.Web.Models;

public class Order : BaseEntity
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int StoreId { get; set; }

    public int? DeliveryDriverId { get; set; }

    [Required]
    public int OrderStatusId { get; set; }

    [Required]
    public int AddressId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public DateTime? DeliveryDate { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    [ForeignKey(nameof(StoreId))]
    public Store Store { get; set; } = null!;

    [ForeignKey(nameof(DeliveryDriverId))]
    public DeliveryDriver? DeliveryDriver { get; set; }

    [ForeignKey(nameof(OrderStatusId))]
    public OrderStatus OrderStatus { get; set; } = null!;

    [ForeignKey(nameof(AddressId))]
    public Address Address { get; set; } = null!;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public Payment? Payment { get; set; }
}
