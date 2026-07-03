using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Persistence.Configurations;

public class PositionHistoryConfiguration : IEntityTypeConfiguration<PositionHistory>
{
    public void Configure(EntityTypeBuilder<PositionHistory> builder)
    {
        builder.ToTable("position_history");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Position)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.StartDate).IsRequired();
    }
}
