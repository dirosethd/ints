using System;
using System.Collections.Generic;

namespace ints.Models;

public partial class FuelType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal PricePerLiter { get; set; }

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
