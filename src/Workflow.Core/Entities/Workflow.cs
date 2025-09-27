using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Core.Entities;

public class WorkFlow
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
}

