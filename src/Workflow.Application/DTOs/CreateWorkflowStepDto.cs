using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Application.DTOs
{
    public class CreateWorkflowStepDto
    {
        public string StepName { get; set; } = null!;
        public string AssignedTo { get; set; } = null!;
        public string ActionType { get; set; } = "input";
        public string? NextStep { get; set; }
        public bool RequiresValidation { get; set; } = false;
    }
}
