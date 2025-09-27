namespace Workflow.Application.Interfaces;

public interface IExternalValidationService
{
    Task<(bool IsValid, string Message)> ValidateAsync(string stepName, object? payload, CancellationToken ct = default);
}
