using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Task
{
    public int Id { get; set; }

    public int DeveloperId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual Developer Developer { get; set; } = null!;

    public virtual ICollection<TaskExecutor> TaskExecutors { get; } = new List<TaskExecutor>();
}
