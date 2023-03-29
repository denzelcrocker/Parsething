using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Region
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int Distance { get; set; }

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}
