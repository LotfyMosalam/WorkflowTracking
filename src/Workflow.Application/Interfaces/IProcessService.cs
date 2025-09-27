using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflow.Application.DTOs;
using Workflow.Core.Entities;

namespace Workflow.Application.Interfaces;

public interface IProcessService
{
    Task<Process> StartProcessAsync(StartProcessDto dto, CancellationToken ct = default);
    Task<ProcessStep> ExecuteStepAsync(ExecuteStepDto dto, CancellationToken ct = default);
    Task<IEnumerable<Process>> QueryProcessesAsync(Guid? workflowId = null, string? status = null, string? assignedTo = null, CancellationToken ct = default);
    Task<Process?> GetProcessByIdAsync(Guid processId, CancellationToken ct = default);
}

