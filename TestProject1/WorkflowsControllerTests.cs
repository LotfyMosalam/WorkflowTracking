using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Workflow.Api.Controllers;
using Workflow.Application.DTOs;
using Workflow.Application.Interfaces;
using Workflow.Core.Entities;

namespace Workflow.Api.Tests.Controllers
{
    public class WorkflowsControllerTests
    {
        private readonly Mock<IWorkflowService> _workflowServiceMock;
        private readonly Mock<ILogger<WorkflowsController>> _loggerMock;
        private readonly WorkflowsController _controller;

        public WorkflowsControllerTests()
        {
            _workflowServiceMock = new Mock<IWorkflowService>();
            _loggerMock = new Mock<ILogger<WorkflowsController>>();
            _controller = new WorkflowsController(_workflowServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenDtoIsValid()
        {
            // Arrange
            var dto = new CreateWorkflowDto { Name = "Test WF" };
            var workflow = new WorkFlow
            {
                Id = Guid.NewGuid(),
                Name = "Test WF"
            };

            _workflowServiceMock
                .Setup(s => s.CreateWorkflowAsync(dto, default))
                .ReturnsAsync(workflow);

            // Act
            var result = await _controller.Create(dto, default);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedWf = Assert.IsType<WorkFlow>(createdResult.Value);
            Assert.Equal(workflow.Id, returnedWf.Id);
            Assert.Equal(workflow.Name, returnedWf.Name);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            CreateWorkflowDto dto = null!; // invalid

            // Act
            var result = await _controller.Create(dto, default);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenWorkflowExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var workflow = new WorkFlow { Id = id, Name = "Existing WF" };

            _workflowServiceMock
                .Setup(s => s.GetWorkflowByIdAsync(id, default))
                .ReturnsAsync(workflow);

            // Act
            var result = await _controller.GetById(id, default);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedWf = Assert.IsType<WorkFlow>(okResult.Value);
            Assert.Equal(workflow.Id, returnedWf.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenWorkflowDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();

            _workflowServiceMock
                .Setup(s => s.GetWorkflowByIdAsync(id, default))
                .ReturnsAsync((WorkFlow?)null);

            // Act
            var result = await _controller.GetById(id, default);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
