using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<EmployeeShift> EmployeeShifts { get; } = new List<EmployeeShift>();

    public virtual ICollection<LoyaltyProgram> LoyaltyPrograms { get; } = new List<LoyaltyProgram>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
