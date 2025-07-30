using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderType { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public int? UserId { get; set; }

    public virtual ICollection<Billing> Billings { get; } = new List<Billing>();

    public virtual ICollection<Kot> Kots { get; } = new List<Kot>();

    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public virtual User? User { get; set; }

    public virtual ICollection<WaitTime> WaitTimes { get; } = new List<WaitTime>();
}
