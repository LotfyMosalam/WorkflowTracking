using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Workflow.Application.Interfaces;
using Workflow.Application.Services;
using Workflow.Infrastructure.Data;
using Workflow.Infrastructure.Repositories;
using Workflow.Infrastructure.Services.ExternalValidation;
using WorkflowTrackingSystem.API.Middlewares;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


builder.Services.AddDbContext<WorkflowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddScoped<IProcessRepository, ProcessRepository>();

builder.Services.AddSingleton<IExternalValidationService, SimulatedExternalValidationService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
