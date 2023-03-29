using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Tag
{
    public int Id { get; set; }

    public string Keyword { get; set; } = null!;
}
