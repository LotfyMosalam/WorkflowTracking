using Microsoft.EntityFrameworkCore;
using Workflow.Application.Interfaces;
using Workflow.Core.Entities;
using Workflow.Infrastructure.Data;

namespace Workflow.Infrastructure.Repositories;

public class ProcessRepository : IProcessRepository
{
    private readonly WorkflowDbContext _db;

    public ProcessRepository(WorkflowDbContext db)
    {
        _db = db;
    }

    public async Task<Process?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Processes
            .Include(p => p.Steps)
            .Include(p => p.Workflow)
                .ThenInclude(w => w.Steps)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task AddAsync(Process process, CancellationToken ct = default)
    {
        await _db.Processes.AddAsync(process, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }

    public IQueryable<Process> Query()
    {
        return _db.Processes
            .Include(p => p.Workflow)
            .Include(p => p.Steps)
            .AsQueryable();
    }
}
