using Microsoft.EntityFrameworkCore;
using Workflow.Application.Interfaces;
using Workflow.Core.Entities;
using Workflow.Infrastructure.Data;

namespace Workflow.Infrastructure.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly WorkflowDbContext _db;

    public WorkflowRepository(WorkflowDbContext db)
    {
        _db = db;
    }

    public async Task<WorkFlow?> GetByIdWithStepsAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Workflows.Include(w => w.Steps).FirstOrDefaultAsync(w => w.Id == id, ct);
    }

    public async Task<WorkFlow> CreateAsync(WorkFlow workflow, CancellationToken ct = default)
    {
        _db.Workflows.Add(workflow);
        await _db.SaveChangesAsync(ct);
        return workflow;
    }

}



  

    

  