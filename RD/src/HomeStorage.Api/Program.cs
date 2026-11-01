using HomeStorage.Application.Interfaces;
using HomeStorage.Application.Services;
using HomeStorage.Domain.Constants;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using HomeStorage.Api.Authorization;
using HomeStorage.Api.Middlewares;
using HomeStorage.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// 1. 读取配置文件里的数据库连接字符串，并注册 AppDbContext 数据库上下文
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

// 从配置中获取 JWT 配置项
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? "YourSuperSecretKeyHereAtLeast32BytesLong!";


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "HomeStorageApi",
        ValidAudience = jwtSettings["Audience"] ?? "HomeStorageClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// 2. 注册依赖注入（Scoped 作用域：每次 HTTP 请求创建一个新实例）
// 告诉系统：当有人要 IItemRepository 时，给它 new 一个 ItemRepository
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

// 3. 注册工作单元（Scoped）
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 告诉系统：当有人要 IItemService 时，给它 new 一个 ItemService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserScopeProvider, UserScopeProvider>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(); // 开启 Swagger 可视化测试页面
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeStorage API", Version = "v1" });

    // 1. 定义 Bearer Token 头部规则
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "请输入 JWT Token，格式为：Bearer {your_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. 将鉴权规则全局应用到 Swagger 测试界面
    // Microsoft.OpenApi 2.x 起：
    //   - security requirement 的键改为 OpenApiSecuritySchemeReference（按「安全方案 Id」引用）；
    //   - Swashbuckle 10 的 AddSecurityRequirement 改为接收工厂委托，以便把宿主文档传给引用对象。
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});
// 4. 功能权限（P2-2）：为每个权限点注册同名授权策略（策略名 == PermissionCodes 中的编码）。
//    处理器内部放行 Admin，其余角色校验 JWT 中的 "Permission" Claim。
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    foreach (var permissionCode in PermissionCodes.All)
    {
        options.AddPolicy(permissionCode, policy => policy
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permissionCode)));
    }
});

var app = builder.Build();

// 开发环境：启动时自动应用迁移并写入种子数据。
// 种子逻辑是幂等的，重复启动不会产生重复数据，也不会覆盖手工调整过的授权。
if (app.Environment.IsDevelopment())
{
    await DatabaseSeeder.InitializeAsync(app.Services);
}

// 注入全局异常拦截中间件（必须放在最靠前的位置）
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 3. 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // 在开发环境下开启 Swagger UI 界面
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();