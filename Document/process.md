🏠 家用物品存储管理系统 (HomeStorage) 开发手册
本手册记录了基于 .NET 8、DDD 领域驱动设计、Clean Architecture（整洁架构） 以及 PostgreSQL 数据库的系统搭建与开发全流程。

📐 1. 项目架构设计与命名规范
1.1 分层架构说明
系统划分为 4 个核心项目层，依赖方向严格遵循从外向内依赖：

HomeStorage.Domain（领域层）：核心业务实体与仓储接口，零外部依赖。

HomeStorage.Application（应用层）：DTO 映射、业务服务逻辑（Services），依赖 Domain。

HomeStorage.Infrastructure（基础设施层）：数据库上下文（DbContext）、EF Core 仓储实现、外部服务，依赖 Application 与 Domain。

HomeStorage.Api（表现/控制层）：RESTful API 控制器、Swagger 配置、依赖注入入口。

1.2 命名规范速查
PascalCase（大驼峰）：解决方案、项目、类、接口、方法、属性（如 HomeStorage.Domain、Item.cs）。

camelCase（小驼峰）：局部变量、方法参数（如 quantity）。

_camelCase（下划线+小驼峰）：类内部私有变量（如 private readonly AppDbContext _context;）。

I + PascalCase：接口规范（如 IItemRepository）。

🛠️ 2. 环境配置与项目创建
2.1 创建解决方案与项目
在终端根目录下依次运行以下命令：

Bash
# 1. 创建解决方案
dotnet new sln -n HomeStorage

# 2. 创建各层级项目
dotnet new classlib -o src/HomeStorage.Domain
dotnet new classlib -o src/HomeStorage.Application
dotnet new classlib -o src/HomeStorage.Infrastructure
dotnet new webapi -o src/HomeStorage.Api

# 3. 将项目添加到解决方案
dotnet sln add src/HomeStorage.Domain/HomeStorage.Domain.csproj
dotnet sln add src/HomeStorage.Application/HomeStorage.Application.csproj
dotnet sln add src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj
dotnet sln add src/HomeStorage.Api/HomeStorage.Api.csproj

# 4. 配置项目间依赖关系
dotnet add src/HomeStorage.Application/HomeStorage.Application.csproj reference src/HomeStorage.Domain/HomeStorage.Domain.csproj
dotnet add src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj reference src/HomeStorage.Application/HomeStorage.Application.csproj
dotnet add src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj reference src/HomeStorage.Domain/HomeStorage.Domain.csproj
dotnet add src/HomeStorage.Api/HomeStorage.Api.csproj reference src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj
dotnet add src/HomeStorage.Api/HomeStorage.Api.csproj reference src/HomeStorage.Application/HomeStorage.Application.csproj
2.2 安装 .NET 8 兼容依赖包
⚠️ 注意：项目使用 .NET 8，EF Core 扩展包必须显式指定版本 -v 8.0.11，避免版本冲突。

Bash
# 1. 为 Infrastructure 安装 PostgreSQL 驱动与设计时工具
dotnet add src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL -v 8.0.11
dotnet add src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design -v 8.0.11

# 2. 为 Api 安装设计时工具（用于 Migration 命令支持）
dotnet add src/HomeStorage.Api/HomeStorage.Api.csproj package Microsoft.EntityFrameworkCore.Design -v 8.0.11
💻 3. 核心代码实现
3.1 领域层 (Domain)
src/HomeStorage.Domain/Entities/Item.cs（充血领域模型）
C#
namespace HomeStorage.Domain.Entities;

public class Item
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string? Location { get; private set; }
    public int Quantity { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Item() { }

    public static Item Create(string name, string category, string? location, int quantity, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("物品名称不能为空", nameof(name));
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("分类不能为空", nameof(category));
        if (quantity < 0) throw new ArgumentException("库存数量不能为负数", nameof(quantity));

        return new Item
        {
            Name = name,
            Category = category,
            Location = location,
            Quantity = quantity,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateInfo(string name, string category, string? location, int quantity, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("物品名称不能为空", nameof(name));
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("分类不能为空", nameof(category));
        if (quantity < 0) throw new ArgumentException("库存数量不能为负数", nameof(quantity));

        Name = name;
        Category = category;
        Location = location;
        Quantity = quantity;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
src/HomeStorage.Domain/Interfaces/IRepository.cs（泛型仓储基类接口）
C#
using System.Linq.Expressions;

namespace HomeStorage.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
src/HomeStorage.Domain/Interfaces/IItemRepository.cs（物品仓储特定接口）
C#
using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    Task<IEnumerable<string>> GetDistinctLocationsAsync();
    Task<bool> SaveChangesAsync();
}
3.2 基础设施层 (Infrastructure)
src/HomeStorage.Infrastructure/Data/AppDbContext.cs
C#
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

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(100);
            
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Location);
        });
    }
}
src/HomeStorage.Infrastructure/Repositories/Repository.cs（通用仓储实现）
C#
using System.Linq.Expressions;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) 
        => await _context.Set<T>().Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Delete(T entity) => _context.Set<T>().Remove(entity);
}
src/HomeStorage.Infrastructure/Repositories/ItemRepository.cs
C#
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<string>> GetDistinctLocationsAsync()
    {
        return await _context.Items
            .Where(i => !string.IsNullOrEmpty(i.Location))
            .Select(i => i.Location!)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
3.3 应用层 (Application)
src/HomeStorage.Application/Dtos/ItemResponseDto.cs
C#
namespace HomeStorage.Application.Dtos;

public class ItemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
src/HomeStorage.Application/Dtos/CreateItemDto.cs
C#
using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class CreateItemDto
{
    [Required(ErrorMessage = "物品名称不能为空")]
    [MaxLength(100, ErrorMessage = "物品名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "分类不能为空")]
    [MaxLength(50, ErrorMessage = "分类名称长度不能超过50个字符")]
    public string Category { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "存放位置长度不能超过100个字符")]
    public string? Location { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "库存数量不能为负数")]
    public int Quantity { get; set; }

    public string? ImageUrl { get; set; }
}
src/HomeStorage.Application/Dtos/UpdateItemDto.cs
C#
using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class UpdateItemDto
{
    [Required(ErrorMessage = "物品名称不能为空")]
    [MaxLength(100, ErrorMessage = "物品名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "分类不能为空")]
    [MaxLength(50, ErrorMessage = "分类名称长度不能超过50个字符")]
    public string Category { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "存放位置长度不能超过100个字符")]
    public string? Location { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "库存数量不能为负数")]
    public int Quantity { get; set; }

    public string? ImageUrl { get; set; }
}
src/HomeStorage.Application/Interfaces/IItemService.cs
C#
using HomeStorage.Application.Dtos;

namespace HomeStorage.Application.Interfaces;

public interface IItemService
{
    Task<IEnumerable<ItemResponseDto>> GetAllItemsAsync();
    Task<ItemResponseDto?> GetItemByIdAsync(int id);
    Task<IEnumerable<string>> GetLocationsAsync();
    Task<ItemResponseDto> CreateItemAsync(CreateItemDto dto);
    Task<ItemResponseDto?> UpdateItemAsync(int id, UpdateItemDto dto);
    Task<bool> DeleteItemAsync(int id);
}
src/HomeStorage.Application/Services/ItemService.cs
C#
using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<IEnumerable<ItemResponseDto>> GetAllItemsAsync()
    {
        var items = await _itemRepository.GetAllAsync();
        return items.Select(MapToResponseDto);
    }

    public async Task<ItemResponseDto?> GetItemByIdAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        return item == null ? null : MapToResponseDto(item);
    }

    public async Task<IEnumerable<string>> GetLocationsAsync()
    {
        return await _itemRepository.GetDistinctLocationsAsync();
    }

    public async Task<ItemResponseDto> CreateItemAsync(CreateItemDto dto)
    {
        var item = Item.Create(dto.Name, dto.Category, dto.Location, dto.Quantity, dto.ImageUrl);
        await _itemRepository.AddAsync(item);
        await _itemRepository.SaveChangesAsync();
        return MapToResponseDto(item);
    }

    public async Task<ItemResponseDto?> UpdateItemAsync(int id, UpdateItemDto dto)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null) return null;

        item.UpdateInfo(dto.Name, dto.Category, dto.Location, dto.Quantity, dto.ImageUrl);
        _itemRepository.Update(item);
        await _itemRepository.SaveChangesAsync();

        return MapToResponseDto(item);
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null) return false;

        _itemRepository.Delete(item);
        return await _itemRepository.SaveChangesAsync();
    }

    private static ItemResponseDto MapToResponseDto(Item item)
    {
        return new ItemResponseDto
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category,
            Location = item.Location,
            Quantity = item.Quantity,
            ImageUrl = item.ImageUrl,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
3.4 表现层 (Api)
src/HomeStorage.Api/appsettings.json（配置 PostgreSQL 连接）
JSON
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=HomeStorageDb;Username=postgres;Password=你的本地密码"
  }
}
src/HomeStorage.Api/Program.cs（配置依赖注入生命周期）
C#
using HomeStorage.Application.Interfaces;
using HomeStorage.Application.Services;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using HomeStorage.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 获取连接字符串并注册 AppDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. 注册依赖注入（Scoped 作用域）
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
🗄️ 4. 数据库迁移与操作命令
在终端中确保代码编译无误（先运行 dotnet build）后，使用以下命令同步数据库结构：

Bash
# 1. 生成数据库迁移文件
dotnet ef migrations add InitialCreate --project src/HomeStorage.Infrastructure --startup-project src/HomeStorage.Api

# 2. 将迁移更新到 PostgreSQL 数据库（自动建库与建表）
dotnet ef database update --project src/HomeStorage.Infrastructure --startup-project src/HomeStorage.Api
整体结构已梳理完成！保存好此文档后，我们就可以正式编写 ItemsController.cs 并启动项目了。