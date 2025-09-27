using Microsoft.Extensions.Logging;
using Workflow.Application.DTOs;
using Workflow.Application.Interfaces;
using Workflow.Core.Entities;

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowRepository _repository;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(IWorkflowRepository repository, ILogger<WorkflowService> logger)
    {
        _repository = repository;
        _logger = logger;
    }


    public async Task<WorkFlow?> GetWorkflowByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _repository.GetByIdWithStepsAsync(id, ct);
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
                RequiresValidation = s.RequiresValidation
            });
        }

        var created = await _repository.CreateAsync(wf, ct);
        _logger.LogInformation("Created workflow {id}", created.Id);
        return created;
    }

}
