using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorage.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RoleCode)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.RoleName)
            .IsRequired()
            .HasMaxLength(100);

        // 配置 User 与 Role 的 1:N 关系
        builder.HasMany(r => r.Users)
               .WithOne(u => u.Role)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}