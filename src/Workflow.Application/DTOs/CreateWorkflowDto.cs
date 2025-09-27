using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Application.DTOs
{
    public class CreateWorkflowDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<CreateWorkflowStepDto> Steps { get; set; } = new();
    }
}
