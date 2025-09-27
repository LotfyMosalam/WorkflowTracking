using Microsoft.AspNetCore.Mvc;
using Workflow.Application.Interfaces;
using Workflow.Application.DTOs;

namespace Workflow.Api.Controllers;

[ApiController]
[Route("v1/workflows")]
public class WorkflowsController : ControllerBase
{
    private readonly IWorkflowService _workflowService;
    private readonly ILogger<WorkflowsController> _logger;

    public WorkflowsController(IWorkflowService workflowService, ILogger<WorkflowsController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkflowDto dto, CancellationToken ct)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Invalid payload.");

        var wf = await _workflowService.CreateWorkflowAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = wf.Id }, wf);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var wf = await _workflowService.GetWorkflowByIdAsync(id, ct);
        if (wf == null) return NotFound();
        return Ok(wf);
    }

}
