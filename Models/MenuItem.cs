using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class MenuItem
{
    public int MenuItemId { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public virtual ICollection<Recipe> Recipes { get; } = new List<Recipe>();
}
