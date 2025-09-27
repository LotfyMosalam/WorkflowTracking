using Workflow.Core.Entities;

namespace Workflow.Application.Interfaces;

public interface IWorkflowRepository
{
    Task<WorkFlow?> GetByIdWithStepsAsync(Guid id, CancellationToken ct = default);
    Task<WorkFlow> CreateAsync(WorkFlow workflow, CancellationToken ct = default);
}

