using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public int QuantityInStock { get; set; }

    public string Unit { get; set; } = null!;

    public int MinStockLevel { get; set; }
}
