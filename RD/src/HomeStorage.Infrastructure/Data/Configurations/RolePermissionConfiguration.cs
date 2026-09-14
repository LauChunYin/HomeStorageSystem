using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorage.Infrastructure.Data.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // 1. 指定表名（可选）
        builder.ToTable("RolePermissions");

        // 2. 配置复合主键（RoleId + PermissionId 组合唯一）
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // 3. 配置与 Role 的一对多关系
        builder.HasOne(rp => rp.Role)
               .WithMany(r => r.RolePermissions)
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade); // 删角色时，级联删除关系记录

        // 4. 配置与 Permission 的一对多关系
        builder.HasOne(rp => rp.Permission)
               .WithMany(p => p.RolePermissions)
               .HasForeignKey(rp => rp.PermissionId)
               .OnDelete(DeleteBehavior.Cascade); // 删权限点时，级联删除关系记录
    }
}