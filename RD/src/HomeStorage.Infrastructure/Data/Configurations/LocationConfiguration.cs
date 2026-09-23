using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorage.Infrastructure.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        // 1. 指定表名
        builder.ToTable("Locations");

        // 2. 主键配置
        builder.HasKey(c => c.Id);

        // 3. 属性约束
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // 位置名称可以加索引，提高按名称搜索的性能
        builder.HasIndex(c => c.Name);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        // 4. 配置自引用一对多关系（父品类与子位置 Parent - Children）
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // 删除父位置时，阻止误删旗下子位置

        // 为父节点外键建立索引，加速树状/层级结构的查询
        builder.HasIndex(c => c.ParentId);

        // 5. 与 Item 的一对多关系（一个位置包含多个物品）
        builder.HasMany(c => c.Items)
            .WithOne(i => i.Location)
            .HasForeignKey(i => i.LocationId)
            .OnDelete(DeleteBehavior.Restrict); // 有物品引用的位置不允许直接删除

        // 6. 与 RoleLocation 的多对多数据范围中间表关系
        builder.HasMany(c => c.RoleLocations)
            .WithOne(rc => rc.Location)
            .HasForeignKey(rc => rc.LocationId)
            .OnDelete(DeleteBehavior.Cascade); // 如果删除了位置，自动清理 RoleLocation 中的对应数据权限记录
    }
}