using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class ProcurementsDocument
{
    public int ProcurementId { get; set; }

    public int DocumentId { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual Procurement Procurement { get; set; } = null!;
}
