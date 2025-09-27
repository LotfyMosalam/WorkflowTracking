using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Core.Entities;

public class ProcessStep
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProcessId { get; set; }
    public Process? Process { get; set; }

    public string StepName { get; set; } = null!;
    public string? PerformedBy { get; set; }             
    public string? Action { get; set; }                  
    public bool IsCompleted { get; set; } = false;
    public DateTime? PerformedAt { get; set; }
    public string? ValidationLog { get; set; }           
}

