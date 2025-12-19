using System;
using System.Collections.Generic;

namespace ints.Models;

public partial class Driver
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public int CarId { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
