using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflow.Core.Entities;
using Workflow.Application.DTOs;

namespace Workflow.Application.Interfaces;

public interface IWorkflowService
{
    Task<WorkFlow> CreateWorkflowAsync(CreateWorkflowDto dto, CancellationToken ct = default);
    Task<WorkFlow?> GetWorkflowByIdAsync(Guid id, CancellationToken ct = default);
}

