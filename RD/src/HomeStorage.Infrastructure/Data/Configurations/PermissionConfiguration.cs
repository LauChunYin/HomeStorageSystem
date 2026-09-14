using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorage.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.PermissionCode)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(100);
    }
}