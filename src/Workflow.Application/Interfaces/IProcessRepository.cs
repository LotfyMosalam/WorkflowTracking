using Workflow.Core.Entities;

namespace Workflow.Application.Interfaces;

public interface IProcessRepository
{
    Task<Process?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Process process, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    IQueryable<Process> Query();
}
