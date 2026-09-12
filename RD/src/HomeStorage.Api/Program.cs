using HomeStorage.Application.Interfaces;
using HomeStorage.Application.Services;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using HomeStorage.Infrastructure.Middlewares;
using HomeStorage.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 读取配置文件里的数据库连接字符串，并注册 AppDbContext 数据库上下文
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. 注册依赖注入（Scoped 作用域：每次 HTTP 请求创建一个新实例）
// 告诉系统：当有人要 IItemRepository 时，给它 new 一个 ItemRepository
builder.Services.AddScoped<IItemRepository, ItemRepository>();

// 告诉系统：当有人要 IItemService 时，给它 new 一个 ItemService
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // 开启 Swagger 可视化测试页面

var app = builder.Build();

// 注入全局异常拦截中间件（必须放在最靠前的位置）
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 3. 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // 在开发环境下开启 Swagger UI 界面
}

app.UseAuthorization();
app.MapControllers();

app.Run();