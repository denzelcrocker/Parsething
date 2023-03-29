using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Component> Components { get; } = new List<Component>();
}
