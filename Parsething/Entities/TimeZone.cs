using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class TimeZone
{
    public int Id { get; set; }

    public string Offset { get; set; } = null!;

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}
