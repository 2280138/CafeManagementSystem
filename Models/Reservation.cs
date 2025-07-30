using System;
using System.Collections.Generic;

namespace CafeManagement.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public string CustomerName { get; set; } = null!;

    public DateTime ReservationDate { get; set; }

    public int NumberOfPeople { get; set; }

    public string Status { get; set; } = null!;
}
