using System;
using System.Collections.Generic;

namespace GasServiceUA.Models;

public partial class MeterReading
{
    public int MeterReadingsId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public long StartMeterReading { get; set; }

    public long EndMeterReading { get; set; }

    public int UsersId { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual User Users { get; set; } = null!;
}
