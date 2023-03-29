using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class ComponentState
{
    public int Id { get; set; }

    public string Kind { get; set; } = null!;

    public virtual ICollection<ComponentCalculation> ComponentCalculations { get; } = new List<ComponentCalculation>();
}
