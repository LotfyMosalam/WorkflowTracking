using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Application.DTOs;

public class StartProcessDto
{
    public Guid WorkflowId { get; set; }
    public string Initiator { get; set; } = null!;
}
