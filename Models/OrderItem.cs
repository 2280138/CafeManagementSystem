using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? MenuItemId { get; set; }

    public int Quantity { get; set; }

    public virtual MenuItem? MenuItem { get; set; }

    public virtual Order? Order { get; set; }
}
