using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Expense
{
    public int ExpenseId { get; set; }

    public DateTime Date { get; set; }

    public string Description { get; set; } = null!;

    public decimal Amount { get; set; }
}
