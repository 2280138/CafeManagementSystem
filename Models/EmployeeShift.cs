using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class EmployeeShift
{
    public int ShiftId { get; set; }

    public int? UserId { get; set; }

    public DateTime ShiftDate { get; set; }

    public string ShiftTime { get; set; } = null!;

    public string Task { get; set; } = null!;

    public virtual User? User { get; set; }
}
