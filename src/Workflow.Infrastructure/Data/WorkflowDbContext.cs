using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.Core.Entities;

namespace Workflow.Infrastructure.Data;

public class WorkflowDbContext : DbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options) { }

    public DbSet<WorkFlow> Workflows => Set<WorkFlow>();
    public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
    public DbSet<Process> Processes => Set<Process>();
    public DbSet<ProcessStep> ProcessSteps => Set<ProcessStep>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkFlow>(b =>
        {
            b.HasKey(w => w.Id);
            b.Property(w => w.Name).IsRequired().HasMaxLength(200);
            b.Property(w => w.Description).HasMaxLength(1000);
            b.HasMany(w => w.Steps).WithOne(s => s.Workflow).HasForeignKey(s => s.WorkflowId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkflowStep>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.StepName).IsRequired().HasMaxLength(200);
            b.Property(s => s.AssignedTo).IsRequired().HasMaxLength(100);
            b.Property<int>("Order");    // Shadow Property
        });

        modelBuilder.Entity<Process>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Initiator).IsRequired().HasMaxLength(200);
            b.HasMany(p => p.Steps).WithOne(s => s.Process).HasForeignKey(s => s.ProcessId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProcessStep>(b =>
        {
            b.HasKey(ps => ps.Id);
            b.Property(ps => ps.StepName).IsRequired().HasMaxLength(200);
        });
    }
}
