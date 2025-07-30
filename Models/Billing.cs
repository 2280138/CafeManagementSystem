using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Billing
{
    public int BillId { get; set; }

    public int? OrderId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal AmountPaid { get; set; }

    public virtual Order? Order { get; set; }
}
