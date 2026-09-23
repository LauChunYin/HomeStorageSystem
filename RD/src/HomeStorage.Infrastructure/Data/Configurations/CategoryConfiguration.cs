using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorage.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // 1. 指定表名
        builder.ToTable("Categories");

        // 2. 主键配置
        builder.HasKey(c => c.Id);

        // 3. 属性约束
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // 品类名称可以加索引，提高按名称搜索的性能
        builder.HasIndex(c => c.Name);

        // 4. 配置自引用一对多关系（父品类与子品类 Parent - Children）
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // 删除父品类时，阻止误删旗下子品类

        // 为父节点外键建立索引，加速树状/层级结构的查询
        builder.HasIndex(c => c.ParentId);

        // 5. 与 Item 的一对多关系（一个品类包含多个物品）
        builder.HasMany(c => c.Items)
            .WithOne(i => i.Category)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // 有物品引用的品类不允许直接删除

        // 6. 与 RoleCategory 的多对多数据范围中间表关系
        builder.HasMany(c => c.RoleCategories)
            .WithOne(rc => rc.Category)
            .HasForeignKey(rc => rc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade); // 如果删除了品类，自动清理 RoleCategory 中的对应数据权限记录
    }
}