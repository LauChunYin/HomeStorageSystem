using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 使用 Fluent API 进行强类型映射与索引优化
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(100);
            
            // 为高频查询字段建立索引，提高查询效率
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Location);
        });
    }
}