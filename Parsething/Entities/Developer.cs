using System;
using System.Collections.Generic;

namespace Parsething.Entities;

public partial class Developer
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public virtual ICollection<TaskExecutor> TaskExecutors { get; } = new List<TaskExecutor>();

    public virtual ICollection<Task> Tasks { get; } = new List<Task>();
}
