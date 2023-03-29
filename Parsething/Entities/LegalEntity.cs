using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class LegalEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}
