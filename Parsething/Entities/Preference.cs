using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Preference
{
    public int Id { get; set; }

    public string Kind { get; set; } = null!;
}
