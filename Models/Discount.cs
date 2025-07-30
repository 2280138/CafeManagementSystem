using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public int Percentage { get; set; }
}
