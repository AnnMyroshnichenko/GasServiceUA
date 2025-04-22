using System;
using System.Collections.Generic;

namespace GasServiceUA.Models;

public partial class Payment
{
    public int PaymentsId { get; set; }

    public int UsersId { get; set; }

    public float Sum { get; set; }

    public DateTime Date { get; set; }

    public virtual User Users { get; set; } = null!;
}
