using System;
using System.Collections.Generic;

namespace GasServiceUA.Models;

public partial class Tariff
{
    public int TariffsId { get; set; }

    public string Name { get; set; } = null!;

    public float CostPerGasUnit { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
