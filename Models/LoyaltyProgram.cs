using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class LoyaltyProgram
{
    public int LoyaltyId { get; set; }

    public int? UserId { get; set; }

    public int Points { get; set; }

    public virtual User? User { get; set; }
}
