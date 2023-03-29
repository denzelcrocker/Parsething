using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Status
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<TaskExecutor> TaskExecutors { get; } = new List<TaskExecutor>();
}
