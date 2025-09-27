using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Core.Entities;

public class WorkflowStep
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StepName { get; set; } = null!;         
    public string AssignedTo { get; set; } = null!;      
    public ActionType ActionType { get; set; } = ActionType.Input;
    public string? NextStep { get; set; }                 
    public int Order { get; set; }                        
    public Guid WorkflowId { get; set; }
    public WorkFlow? Workflow { get; set; }
    public bool RequiresValidation { get; set; } = false;
}

