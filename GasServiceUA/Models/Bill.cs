using System;
using System.Collections.Generic;

namespace GasServiceUA.Models;

public partial class Bill
{
    public int BillsId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public float Cost { get; set; }

    public int UsersId { get; set; }

    public int? MeterReadingsId { get; set; }

    public virtual MeterReading? MeterReadings { get; set; }

    public virtual User Users { get; set; } = null!;
}
