using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class TaskExecutor
{
    public int Id { get; set; }

    public int DeveloperId { get; set; }

    public int TaskId { get; set; }

    public int StatusId { get; set; }

    public virtual Developer Developer { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
