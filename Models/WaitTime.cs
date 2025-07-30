using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class WaitTime
{
    public int WaitTimeId { get; set; }

    public int? OrderId { get; set; }

    public int EstimatedTime { get; set; }

    public virtual Order? Order { get; set; }
}
