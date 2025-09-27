using Microsoft.AspNetCore.Mvc;
using Workflow.Application.Interfaces;
using Workflow.Application.DTOs;

namespace Workflow.Api.Controllers;

[ApiController]
[Route("v1/processes")]
public class ProcessesController : ControllerBase
{
    private readonly IProcessService _processService;
    private readonly ILogger<ProcessesController> _logger;

    public ProcessesController(IProcessService processService, ILogger<ProcessesController> logger)
    {
        _processService = processService;
        _logger = logger;
    }


    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartProcessDto dto, CancellationToken ct)
    {
        try
        {
            var p = await _processService.StartProcessAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }


    [HttpPost("execute")]
    public async Task<IActionResult> Execute([FromBody] ExecuteStepDto dto, CancellationToken ct)
    {
        try
        {
            var step = await _processService.ExecuteStepAsync(dto, ct);
            return Ok(step);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // this covers validation failures and step/order problems
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpGet]
    public async Task<IActionResult> Query([FromQuery] Guid? workflow_id, [FromQuery] string? status, [FromQuery] string? assigned_to, CancellationToken ct)
    {
        var list = await _processService.QueryProcessesAsync(workflow_id, status, assigned_to, ct);
        return Ok(list);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var p = await _processService.GetProcessByIdAsync(id, ct);
        if (p == null) return NotFound();
        return Ok(p);
    }
}
