using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Seller
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ComponentCalculation> ComponentCalculations { get; } = new List<ComponentCalculation>();
}
