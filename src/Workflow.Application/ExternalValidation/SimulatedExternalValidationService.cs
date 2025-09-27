using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflow.Application.Interfaces;


namespace Workflow.Infrastructure.Services.ExternalValidation;

public class SimulatedExternalValidationService : IExternalValidationService
{
    private readonly ILogger<SimulatedExternalValidationService> _logger;

    public SimulatedExternalValidationService(ILogger<SimulatedExternalValidationService> logger)
    {
        _logger = logger;
    }

    public Task<(bool IsValid, string Message)> ValidateAsync(string stepName, object? payload, CancellationToken ct = default)
    {
        try
        {
            var name = stepName ?? string.Empty;
            if (name.IndexOf("ExternalFail", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                _logger.LogInformation("Simulated validation: forced fail for step {step}", stepName);
                return Task.FromResult((false, "External validation forced failure for testing."));
            }

            if (name.IndexOf("Finance", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (payload == null)
                    return Task.FromResult((false, "Finance validation requires payload with amount."));

                try
                {
                    if (payload is System.Text.Json.JsonElement je && je.TryGetProperty("amount", out var amtEl))
                    {
                        if (amtEl.TryGetDecimal(out var dec) && dec > 10000m)
                            return Task.FromResult((false, $"Amount {dec} exceeds allowed threshold."));
                        return Task.FromResult((true, "Amount validated."));
                    }

                    var type = payload.GetType();
                    var prop = type.GetProperty("amount");
                    if (prop != null)
                    {
                        var val = prop.GetValue(payload);
                        if (val is IConvertible conv)
                        {
                            var dec = Convert.ToDecimal(conv);
                            if (dec > 10000m)
                                return Task.FromResult((false, $"Amount {dec} exceeds allowed threshold."));
                            return Task.FromResult((true, "Amount validated."));
                        }
                    }

                    return Task.FromResult((true, "Finance validation assumed valid (no numeric amount found)."));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing payload for finance validation");
                    return Task.FromResult((false, "Error parsing payload for finance validation."));
                }
            }

            return Task.FromResult((true, "Validation passed (simulated)."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External validation simulation error");
            return Task.FromResult((false, $"Validation error: {ex.Message}"));
        }
    }
}

