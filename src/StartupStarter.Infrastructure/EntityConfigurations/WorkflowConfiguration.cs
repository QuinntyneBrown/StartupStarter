using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.HasKey(e => e.WorkflowId);

        builder.Property(e => e.WorkflowId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ContentId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.WorkflowType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.InitiatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.CurrentAssigneeId)
            .HasMaxLength(100);

        builder.Property(e => e.CurrentStage)
            .HasMaxLength(100);

        builder.Property(e => e.FinalStatus)
            .HasMaxLength(100);

        builder.Property(e => e.StartedAt)
            .IsRequired();

        builder.Property(e => e.IsCompleted)
            .IsRequired();

        // Configure one-to-many relationship with WorkflowStages
        builder.HasMany(e => e.Stages)
            .WithOne(s => s.Workflow)
            .HasForeignKey(s => s.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship with WorkflowApprovals
        builder.HasMany(e => e.Approvals)
            .WithOne(a => a.Workflow)
            .HasForeignKey(a => a.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore DomainEvents collection
        builder.Ignore(e => e.DomainEvents);

        builder.HasIndex(e => e.ContentId);
        builder.HasIndex(e => e.AccountId);
        builder.HasIndex(e => e.InitiatedBy);
        builder.HasIndex(e => e.IsCompleted);
    }
}
