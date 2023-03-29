using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class WarrantyState
{
    public int Id { get; set; }

    public string Kind { get; set; } = null!;

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}
