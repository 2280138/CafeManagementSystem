using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public int? MenuItemId { get; set; }

    public string Ingredient { get; set; } = null!;

    public string QuantityRequired { get; set; } = null!;

    public virtual MenuItem? MenuItem { get; set; }
}
