using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class WorkflowStageConfiguration : IEntityTypeConfiguration<WorkflowStage>
{
    public void Configure(EntityTypeBuilder<WorkflowStage> builder)
    {
        builder.HasKey(e => e.WorkflowStageId);

        builder.Property(e => e.WorkflowStageId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.WorkflowId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.StageName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.StageOrder)
            .IsRequired();

        builder.Property(e => e.CompletedBy)
            .HasMaxLength(100);

        builder.Property(e => e.IsCompleted)
            .IsRequired();

        // Configure many-to-one relationship with Workflow
        builder.HasOne(e => e.Workflow)
            .WithMany(w => w.Stages)
            .HasForeignKey(e => e.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.WorkflowId);
        builder.HasIndex(e => e.StageOrder);
        builder.HasIndex(e => e.IsCompleted);
    }
}
