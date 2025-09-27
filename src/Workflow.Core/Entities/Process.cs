using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Core.Entities;

public class Process
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowId { get; set; }
    public WorkFlow? Workflow { get; set; }

    public string Initiator { get; set; } = null!;                                    
    public ProcessStatus Status { get; set; } = ProcessStatus.Pending;
    public string? CurrentStepName { get; set; }                                     
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProcessStep> Steps { get; set; } = new List<ProcessStep>();
}

