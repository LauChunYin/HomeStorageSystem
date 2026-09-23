using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorage.Infrastructure.Data.Configurations;

public class RoleCategoryConfiguration : IEntityTypeConfiguration<RoleCategory>
{
    public void Configure(EntityTypeBuilder<RoleCategory> builder)
    {
        // 1. 指定表名（可选）
        builder.ToTable("RoleCategories");

        // 2. 配置复合主键（RoleId + CategoryId 组合唯一）
        builder.HasKey(rp => new { rp.RoleId, rp.CategoryId });

        // 3. 配置与 Role 的一对多关系
        builder.HasOne(rp => rp.Role)
               .WithMany(r => r.RoleCategories)
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade); // 删角色时，级联删除关系记录

        // 4. 配置与 Location 的一对多关系
        builder.HasOne(rp => rp.Category)
               .WithMany(p => p.RoleCategories)
               .HasForeignKey(rp => rp.CategoryId)
               .OnDelete(DeleteBehavior.Cascade); // 删权限点时，级联删除关系记录
    }
}