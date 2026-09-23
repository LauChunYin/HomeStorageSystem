using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorage.Infrastructure.Data.Configurations;

public class RoleLocationConfiguration : IEntityTypeConfiguration<RoleLocation>
{
    public void Configure(EntityTypeBuilder<RoleLocation> builder)
    {
        // 1. 指定表名（可选）
        builder.ToTable("RoleLocations");

        // 2. 配置复合主键（RoleId + LocationId 组合唯一）
        builder.HasKey(rp => new { rp.RoleId, rp.LocationId });

        // 3. 配置与 Role 的一对多关系
        builder.HasOne(rp => rp.Role)
               .WithMany(r => r.RoleLocations)
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade); // 删角色时，级联删除关系记录

        // 4. 配置与 Location 的一对多关系
        builder.HasOne(rp => rp.Location)
               .WithMany(p => p.RoleLocations)
               .HasForeignKey(rp => rp.LocationId)
               .OnDelete(DeleteBehavior.Cascade); // 删权限点时，级联删除关系记录
    }
}