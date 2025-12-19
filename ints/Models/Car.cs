using System;
using System.Collections.Generic;

namespace ints.Models;

public partial class Car
{
    public int Id { get; set; }

    public string RegNumber { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
