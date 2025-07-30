using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Kot
{
    public int Kotid { get; set; }

    public int? OrderId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime GeneratedTime { get; set; }

    public virtual Order? Order { get; set; }
}
