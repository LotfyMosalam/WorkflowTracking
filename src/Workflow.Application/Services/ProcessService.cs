using Workflow.Application.DTOs;
using Workflow.Application.Interfaces;
using Workflow.Core.Entities;


namespace Workflow.Application.Services;

public class ProcessService : IProcessService
{
    private readonly IProcessRepository _processRepo;
    private readonly IWorkflowRepository _workflowRepo;
    private readonly IExternalValidationService _validationService;

    public ProcessService(IProcessRepository processRepo, IWorkflowRepository workflowRepo, IExternalValidationService validationService)
    {
        _processRepo = processRepo;
        _workflowRepo = workflowRepo;
        _validationService = validationService;
    }

    public async Task<Process> StartProcessAsync(StartProcessDto dto, CancellationToken ct = default)
    {
        var wf = await _workflowRepo.GetByIdWithStepsAsync(dto.WorkflowId, ct);
        if (wf == null) throw new KeyNotFoundException("Workflow not found.");

        var first = wf.Steps.OrderBy(s => s.Order).FirstOrDefault();
        string? firstName = first?.StepName;

        var process = new Process
        {
            WorkflowId = wf.Id,
            Initiator = dto.Initiator,
            Status = ProcessStatus.Active,
            CurrentStepName = firstName
        };

        if (firstName != null)
        {
            process.Steps.Add(new ProcessStep
            {
                StepName = firstName,
                IsCompleted = false
            });
        }

        await _processRepo.AddAsync(process, ct);
        await _processRepo.SaveChangesAsync(ct);

        return process;
    }

    public async Task<ProcessStep> ExecuteStepAsync(ExecuteStepDto dto, CancellationToken ct = default)
    {
        var process = await _processRepo.GetByIdAsync(dto.ProcessId, ct);
        if (process == null) throw new KeyNotFoundException("Process not found.");

        var wfStep = process.Workflow!.Steps.FirstOrDefault(s => s.StepName == dto.StepName);
        if (wfStep == null) throw new InvalidOperationException("Step not found in workflow definition.");

        if (!string.Equals(process.CurrentStepName, dto.StepName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("This step is not the current active step for the process.");

        string? validationLog = null;
        if (wfStep.RequiresValidation)
        {
            var (isValid, message) = await _validationService.ValidateAsync(dto.StepName, dto.Payload, ct);
            validationLog = message;
            if (!isValid)
            {
                process.Steps.Add(new ProcessStep
                {
                    StepName = dto.StepName,
                    PerformedBy = dto.PerformedBy,
                    Action = dto.Action,
                    IsCompleted = false,
                    PerformedAt = DateTime.UtcNow,
                    ValidationLog = $"Validation failed: {message}"
                });
                await _processRepo.SaveChangesAsync(ct);
                throw new InvalidOperationException($"Validation failed: {message}");
            }
        }

        var currentProcessStep = process.Steps.FirstOrDefault(s => s.StepName == dto.StepName && !s.IsCompleted)
                                ?? new ProcessStep { StepName = dto.StepName };

        currentProcessStep.PerformedBy = dto.PerformedBy;
        currentProcessStep.Action = dto.Action;
        currentProcessStep.IsCompleted = true;
        currentProcessStep.PerformedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(validationLog))
            currentProcessStep.ValidationLog = validationLog;

        if (!process.Steps.Contains(currentProcessStep))
            process.Steps.Add(currentProcessStep);

        var nextStepName = wfStep.NextStep;
        if (string.IsNullOrWhiteSpace(nextStepName) || string.Equals(nextStepName, "Completed", StringComparison.OrdinalIgnoreCase))
        {
            process.CurrentStepName = null;
            process.Status = ProcessStatus.Completed;
        }
        else
        {
            process.CurrentStepName = nextStepName;
            process.Steps.Add(new ProcessStep
            {
                StepName = nextStepName,
                IsCompleted = false
            });
        }

        await _processRepo.SaveChangesAsync(ct);
        return currentProcessStep;
    }

    public async Task<IEnumerable<Process>> QueryProcessesAsync(Guid? workflowId = null, string? status = null, string? assignedTo = null, CancellationToken ct = default)
    {
        var q = _processRepo.Query();

        if (workflowId.HasValue) q = q.Where(p => p.WorkflowId == workflowId.Value);

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ProcessStatus>(status, true, out var st))
            q = q.Where(p => p.Status == st);

        if (!string.IsNullOrWhiteSpace(assignedTo))
            q = q.Where(p => p.Workflow != null && p.Workflow.Steps.Any(s => s.StepName == p.CurrentStepName && s.AssignedTo == assignedTo));

        return await Task.FromResult(q.OrderByDescending(p => p.StartedAt).ToList());
    }

    public async Task<Process?> GetProcessByIdAsync(Guid processId, CancellationToken ct = default)
        => await _processRepo.GetByIdAsync(processId, ct);

}
