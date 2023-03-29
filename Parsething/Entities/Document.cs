using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Document
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
}
