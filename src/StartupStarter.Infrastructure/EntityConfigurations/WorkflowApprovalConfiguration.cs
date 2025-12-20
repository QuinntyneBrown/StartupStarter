using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class WorkflowApprovalConfiguration : IEntityTypeConfiguration<WorkflowApproval>
{
    public void Configure(EntityTypeBuilder<WorkflowApproval> builder)
    {
        builder.HasKey(e => e.WorkflowApprovalId);

        builder.Property(e => e.WorkflowApprovalId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.WorkflowId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ApprovedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ApprovalLevel)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Comments)
            .HasMaxLength(1000);

        builder.Property(e => e.IsApproved)
            .IsRequired();

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(500);

        builder.Property(e => e.ApprovalDate)
            .IsRequired();

        // Configure many-to-one relationship with Workflow
        builder.HasOne(e => e.Workflow)
            .WithMany(w => w.Approvals)
            .HasForeignKey(e => e.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.WorkflowId);
        builder.HasIndex(e => e.ApprovedBy);
        builder.HasIndex(e => e.IsApproved);
        builder.HasIndex(e => e.ApprovalDate);
    }
}
