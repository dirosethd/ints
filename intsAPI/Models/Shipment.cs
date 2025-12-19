using System;
using System.Collections.Generic;

namespace ints.Models;

public partial class Shipment
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string FromLocation { get; set; } = null!;

    public string ToLocation { get; set; } = null!;

    public decimal DistanceKm { get; set; }

    public decimal VolumeLiters { get; set; }

    public int CarId { get; set; }

    public int DriverId { get; set; }

    public int FuelTypeId { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual Driver Driver { get; set; } = null!;

    public virtual FuelType FuelType { get; set; } = null!;
}
