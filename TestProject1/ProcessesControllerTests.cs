using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Workflow.Api.Controllers;
using Workflow.Application.DTOs;
using Workflow.Application.Interfaces;
using Workflow.Core.Entities;
using Xunit;

namespace Workflow.Api.Tests.Controllers
{
    public class ProcessesControllerTests
    {
        private readonly Mock<IProcessService> _processServiceMock;
        private readonly Mock<ILogger<ProcessesController>> _loggerMock;
        private readonly ProcessesController _controller;

        public ProcessesControllerTests()
        {
            _processServiceMock = new Mock<IProcessService>();
            _loggerMock = new Mock<ILogger<ProcessesController>>();
            _controller = new ProcessesController(_processServiceMock.Object, _loggerMock.Object);
        }


        [Fact]
        public async Task Start_ShouldReturnCreated_WhenProcessStarted()
        {
            // Arrange
            var dto = new StartProcessDto { WorkflowId = Guid.NewGuid() };
            var process = new Process
            {
                Id = Guid.NewGuid(),
                WorkflowId = dto.WorkflowId,
                Initiator = "user1",
                Status = ProcessStatus.Pending
            };

            _processServiceMock
                .Setup(s => s.StartProcessAsync(dto, default))
                .ReturnsAsync(process);

            // Act
            var result = await _controller.Start(dto, default);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedProcess = Assert.IsType<Process>(createdResult.Value);
            Assert.Equal(process.Id, returnedProcess.Id);
            Assert.Equal(process.WorkflowId, returnedProcess.WorkflowId);
        }


        [Fact]
        public async Task Start_ShouldReturnNotFound_WhenKeyNotFound()
        {
            var dto = new StartProcessDto { WorkflowId = Guid.NewGuid() };

            _processServiceMock
                .Setup(s => s.StartProcessAsync(dto, default))
                .ThrowsAsync(new KeyNotFoundException("workflow not found"));

            var result = await _controller.Start(dto, default);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("workflow not found", notFoundResult.Value!.ToString());
        }


        [Fact]
        public async Task Execute_ShouldReturnOk_WhenStepExecuted()
        {
            var dto = new ExecuteStepDto { ProcessId = Guid.NewGuid() };
            var step = new ProcessStep
            {
                Id = Guid.NewGuid(),
                ProcessId = dto.ProcessId,
                StepName = "Step 1",
                PerformedBy = "user2",
                IsCompleted = true
            };

            _processServiceMock
                .Setup(s => s.ExecuteStepAsync(dto, default))
                .ReturnsAsync(step);

            var result = await _controller.Execute(dto, default);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStep = Assert.IsType<ProcessStep>(okResult.Value);
            Assert.Equal(step.Id, returnedStep.Id);
            Assert.Equal(step.ProcessId, returnedStep.ProcessId);
        }


        [Fact]
        public async Task Execute_ShouldReturnNotFound_WhenKeyNotFound()
        {
            var dto = new ExecuteStepDto { ProcessId = Guid.NewGuid() };

            _processServiceMock
                .Setup(s => s.ExecuteStepAsync(dto, default))
                .ThrowsAsync(new KeyNotFoundException("process not found"));

            var result = await _controller.Execute(dto, default);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("process not found", notFoundResult.Value!.ToString());
        }


        [Fact]
        public async Task Execute_ShouldReturnBadRequest_WhenInvalidOperation()
        {
            var dto = new ExecuteStepDto { ProcessId = Guid.NewGuid() };

            _processServiceMock
                .Setup(s => s.ExecuteStepAsync(dto, default))
                .ThrowsAsync(new InvalidOperationException("invalid step"));

            var result = await _controller.Execute(dto, default);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("invalid step", badRequestResult.Value!.ToString());
        }


        [Fact]
        public async Task Query_ShouldReturnOk_WithList()
        {
            var list = new List<Process>
            {
                new Process { Id = Guid.NewGuid(), WorkflowId = Guid.NewGuid(), Initiator = "user3" }
            };

            _processServiceMock
                .Setup(s => s.QueryProcessesAsync(null, null, null, default))
                .ReturnsAsync(list);

            var result = await _controller.Query(null, null, null, default);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<Process>>(okResult.Value);
            Assert.Single(returnedList);
        }


        [Fact]
        public async Task GetById_ShouldReturnOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var process = new Process { Id = id, WorkflowId = Guid.NewGuid(), Initiator = "user4" };

            _processServiceMock
                .Setup(s => s.GetProcessByIdAsync(id, default))
                .ReturnsAsync(process);

            var result = await _controller.GetById(id, default);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProcess = Assert.IsType<Process>(okResult.Value);
            Assert.Equal(process.Id, returnedProcess.Id);
        }


        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();

            _processServiceMock
                .Setup(s => s.GetProcessByIdAsync(id, default))
                .ReturnsAsync((Process?)null);

            var result = await _controller.GetById(id, default);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
