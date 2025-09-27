using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflow.Application.DTOs;
using Workflow.Application.Interfaces;
using Workflow.Core.Entities;
using Workflow.Infrastructure.Data;

namespace Workflow.Infrastructure.Services;

public class WorkflowService : IWorkflowService
{
    private readonly WorkflowDbContext _db;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(WorkflowDbContext db, ILogger<WorkflowService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<WorkFlow> CreateWorkflowAsync(CreateWorkflowDto dto, CancellationToken ct = default)
    {
        var wf = new WorkFlow
        {
            Name = dto.Name,
            Description = dto.Description,
        };

        int order = 1;
        foreach (var s in dto.Steps)
        {
            var actionType = s.ActionType?.ToLowerInvariant() switch
            {
                "approve_reject" or "approve-reject" or "approvereject" => ActionType.ApproveReject,
                _ => ActionType.Input
            };

            wf.Steps.Add(new WorkflowStep
            {
                StepName = s.StepName,
                AssignedTo = s.AssignedTo,
                ActionType = actionType,
                NextStep = s.NextStep,
                Order = order++,
                // new property:
                RequiresValidation = s.RequiresValidation
            });
        }

        _db.Workflows.Add(wf);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Created workflow {id}", wf.Id);
        return wf;
    }

    public async Task<WorkFlow?> GetWorkflowByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Workflows.Include(w => w.Steps).FirstOrDefaultAsync(w => w.Id == id, ct);
    }
}

