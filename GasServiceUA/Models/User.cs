using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasServiceUA.Models;

[Table("Users")]
public partial class User : IdentityUser<int>
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public int AccountNumber { get; set; }

    public float Balance { get; set; }

    public string City { get; set; } = null!;

    public string CityDistrict { get; set; } = null!;

    public string Street { get; set; } = null!;

    public short BuildingNumber { get; set; }

    public int TariffsId { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Tariff Tariffs { get; set; } = null!;
}
