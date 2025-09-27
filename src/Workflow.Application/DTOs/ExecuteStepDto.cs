using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Application.DTOs;

public class ExecuteStepDto
{
    public Guid ProcessId { get; set; }
    public string StepName { get; set; } = null!;
    public string PerformedBy { get; set; } = null!;
    public string Action { get; set; } = null!; 
    public object? Payload { get; set; } 
}
