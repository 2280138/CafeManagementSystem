using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? Comments { get; set; }

    public int Rating { get; set; }
}
