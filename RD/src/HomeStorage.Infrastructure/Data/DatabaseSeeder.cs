using HomeStorage.Domain.Constants;
using HomeStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HomeStorage.Infrastructure.Data;

/// <summary>
/// 种子数据初始化器。
/// <para>
/// 所有步骤都是<b>幂等</b>的：以业务唯一键（PermissionCode / RoleCode / UserName / Name）判重，
/// 已存在的数据不会重复插入，也不会覆盖你后来手工修改的授权，因此可安全地在每次启动时执行。
/// </para>
/// </summary>
public static class DatabaseSeeder
{
    // ---------- 角色编码 ----------
    public const string AdminRoleCode = "Admin";
    public const string KeeperRoleCode = "Keeper";
    public const string ViewerRoleCode = "Viewer";

    // ---------- 演示账号（密码均满足 DTO 的强密码策略） ----------
    public const string AdminUserName = "admin";
    public const string AdminPassword = "Admin@12345";
    public const string KeeperUserName = "keeper";
    public const string KeeperPassword = "Keeper@12345";
    public const string ViewerUserName = "viewer";
    public const string ViewerPassword = "Viewer@12345";

    // ---------- 演示品类 / 位置 / 物料 ----------
    private const string FoodCategory = "食品";
    private const string ToolCategory = "工具";
    private const string CleaningCategory = "清洁用品";

    private const string KitchenLocation = "厨房";
    private const string BalconyLocation = "阳台";
    private const string StoreroomLocation = "储物间";

    private static readonly IReadOnlyDictionary<string, string> PermissionDescriptions =
        new Dictionary<string, string>
        {
            [PermissionCodes.ItemCreate] = "新增物料",
            [PermissionCodes.ItemUpdate] = "修改物料",
            [PermissionCodes.ItemDelete] = "删除物料",
            [PermissionCodes.CategoryCreate] = "新增品类",
            [PermissionCodes.CategoryUpdate] = "修改品类",
            [PermissionCodes.CategoryDelete] = "删除品类",
            [PermissionCodes.LocationCreate] = "新增位置",
            [PermissionCodes.LocationUpdate] = "修改位置",
            [PermissionCodes.LocationDelete] = "删除位置",
            [PermissionCodes.UserCreate] = "新增用户",
            [PermissionCodes.UserUpdate] = "修改用户",
            [PermissionCodes.UserDelete] = "删除用户",
            [PermissionCodes.RolePermissionAssign] = "分配角色功能权限",
            [PermissionCodes.RoleCategoryAssign] = "分配角色品类范围",
            [PermissionCodes.RoleLocationAssign] = "分配角色位置范围"
        };

    /// <summary>
    /// 创建作用域、执行数据库迁移并写入种子数据。
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();
        await SeedAsync(db);
    }

    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedPermissionsAsync(db);
        await SeedRolesAsync(db);
        await SeedRolePermissionsAsync(db);
        await SeedCategoriesAsync(db);
        await SeedLocationsAsync(db);
        await SeedDataScopesAsync(db);
        await SeedItemsAsync(db);
        await SeedUsersAsync(db);
    }

    private static async Task SeedPermissionsAsync(AppDbContext db)
    {
        var existingCodes = await db.Permissions.Select(p => p.PermissionCode).ToListAsync();

        var added = false;
        foreach (var code in PermissionCodes.All)
        {
            if (existingCodes.Contains(code, StringComparer.OrdinalIgnoreCase)) continue;

            var description = PermissionDescriptions.TryGetValue(code, out var text) ? text : code;
            db.Permissions.Add(Permission.Create(code, description));
            added = true;
        }

        if (added) await db.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(AppDbContext db)
    {
        var roleDefinitions = new (string Code, string Name)[]
        {
            (AdminRoleCode, "系统管理员"),
            (KeeperRoleCode, "仓储管理员"),
            (ViewerRoleCode, "只读访客")
        };

        var existingCodes = await db.Roles.Select(r => r.RoleCode).ToListAsync();

        var added = false;
        foreach (var (code, name) in roleDefinitions)
        {
            if (existingCodes.Contains(code, StringComparer.OrdinalIgnoreCase)) continue;

            db.Roles.Add(Role.Create(code, name));
            added = true;
        }

        if (added) await db.SaveChangesAsync();
    }

    private static async Task SeedRolePermissionsAsync(AppDbContext db)
    {
        var roleIds = await db.Roles.ToDictionaryAsync(r => r.RoleCode, r => r.Id, StringComparer.OrdinalIgnoreCase);
        var permissionIds = await db.Permissions.ToDictionaryAsync(p => p.PermissionCode, p => p.Id, StringComparer.OrdinalIgnoreCase);

        var grants = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            // 管理员天然拥有全量权限（授权处理器里也直接放行，这里保持数据一致）
            [AdminRoleCode] = PermissionCodes.All.ToArray(),

            // 仓储管理员：可维护物料/品类/位置，但不能管理用户与角色授权
            [KeeperRoleCode] = new[]
            {
                PermissionCodes.ItemCreate,
                PermissionCodes.ItemUpdate,
                PermissionCodes.ItemDelete,
                PermissionCodes.CategoryCreate,
                PermissionCodes.CategoryUpdate,
                PermissionCodes.LocationCreate,
                PermissionCodes.LocationUpdate
            },

            // 只读访客：不授予任何功能权限，只能读取其数据范围内的物料
            [ViewerRoleCode] = Array.Empty<string>()
        };

        var existing = await db.RolePermissions
            .Select(rp => new { rp.RoleId, rp.PermissionId })
            .ToListAsync();

        var added = false;
        foreach (var (roleCode, codes) in grants)
        {
            if (!roleIds.TryGetValue(roleCode, out var roleId)) continue;

            foreach (var code in codes)
            {
                if (!permissionIds.TryGetValue(code, out var permissionId)) continue;

                var alreadyGranted = existing.Any(x => x.RoleId == roleId && x.PermissionId == permissionId);
                if (alreadyGranted) continue;

                db.RolePermissions.Add(RolePermission.Create(roleId, permissionId));
                added = true;
            }
        }

        if (added) await db.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(AppDbContext db)
    {
        var names = new[] { FoodCategory, ToolCategory, CleaningCategory };
        var existing = await db.Categories.Select(c => c.Name).ToListAsync();

        var added = false;
        foreach (var name in names)
        {
            if (existing.Contains(name)) continue;

            db.Categories.Add(Category.Create(name, null));
            added = true;
        }

        if (added) await db.SaveChangesAsync();
    }

    private static async Task SeedLocationsAsync(AppDbContext db)
    {
        var names = new[] { KitchenLocation, BalconyLocation, StoreroomLocation };
        var existing = await db.Locations.Select(l => l.Name).ToListAsync();

        var added = false;
        foreach (var name in names)
        {
            if (existing.Contains(name)) continue;

            db.Locations.Add(Location.Create(name, null));
            added = true;
        }

        if (added) await db.SaveChangesAsync();
    }

    /// <summary>
    /// 给「仓储管理员」分配最小化的数据范围：仅「食品」品类 + 「厨房」位置。
    /// 用于演示 AND 过滤 —— 它只能看到同时满足这两个条件的物料。
    /// 「只读访客」故意不分配任何范围，用于演示「无授权则看不到数据」。
    /// </summary>
    private static async Task SeedDataScopesAsync(AppDbContext db)
    {
        var keeper = await db.Roles.FirstOrDefaultAsync(r => r.RoleCode == KeeperRoleCode);
        if (keeper is null) return;

        var added = false;

        var food = await db.Categories.FirstOrDefaultAsync(c => c.Name == FoodCategory);
        if (food is not null &&
            !await db.RoleCategories.AnyAsync(rc => rc.RoleId == keeper.Id && rc.CategoryId == food.Id))
        {
            db.RoleCategories.Add(RoleCategory.Create(keeper.Id, food.Id));
            added = true;
        }

        var kitchen = await db.Locations.FirstOrDefaultAsync(l => l.Name == KitchenLocation);
        if (kitchen is not null &&
            !await db.RoleLocations.AnyAsync(rl => rl.RoleId == keeper.Id && rl.LocationId == kitchen.Id))
        {
            db.RoleLocations.Add(RoleLocation.Create(keeper.Id, kitchen.Id));
            added = true;
        }

        if (added) await db.SaveChangesAsync();
    }

    private static async Task SeedItemsAsync(AppDbContext db)
    {
        // 物料只在空表时初始化，避免往用户已有数据里塞演示条目
        if (await db.Items.AnyAsync()) return;

        var food = await db.Categories.FirstOrDefaultAsync(c => c.Name == FoodCategory);
        var tool = await db.Categories.FirstOrDefaultAsync(c => c.Name == ToolCategory);
        var cleaning = await db.Categories.FirstOrDefaultAsync(c => c.Name == CleaningCategory);

        var kitchen = await db.Locations.FirstOrDefaultAsync(l => l.Name == KitchenLocation);
        var balcony = await db.Locations.FirstOrDefaultAsync(l => l.Name == BalconyLocation);
        var storeroom = await db.Locations.FirstOrDefaultAsync(l => l.Name == StoreroomLocation);

        if (food is null || tool is null || cleaning is null) return;

        // 食品 + 厨房：Keeper（仅授权 食品/厨房）可见
        if (kitchen is not null)
        {
            db.Items.Add(Item.Create("大米", food.Id, kitchen.Id, null));
            db.Items.Add(Item.Create("酱油", food.Id, kitchen.Id, null));
        }

        // 工具 + 储物间、清洁用品 + 阳台：Keeper 不可见（位置不在授权范围）
        if (storeroom is not null)
            db.Items.Add(Item.Create("扳手", tool.Id, storeroom.Id, null));

        if (balcony is not null)
            db.Items.Add(Item.Create("拖把", cleaning.Id, balcony.Id, null));

        // 无位置物料：按业务决策 3，仅系统管理员可见
        db.Items.Add(Item.Create("备用灯泡", tool.Id, null, null));

        await db.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext db)
    {
        var roleIds = await db.Roles.ToDictionaryAsync(r => r.RoleCode, r => r.Id, StringComparer.OrdinalIgnoreCase);
        var existingUserNames = await db.Users.Select(u => u.UserName).ToListAsync();

        var seedUsers = new (string UserName, string Password, string RoleCode)[]
        {
            (AdminUserName, AdminPassword, AdminRoleCode),
            (KeeperUserName, KeeperPassword, KeeperRoleCode),
            (ViewerUserName, ViewerPassword, ViewerRoleCode)
        };

        var added = false;
        foreach (var (userName, password, roleCode) in seedUsers)
        {
            if (existingUserNames.Contains(userName, StringComparer.OrdinalIgnoreCase)) continue;
            if (!roleIds.TryGetValue(roleCode, out var roleId)) continue;

            // 口令以 BCrypt 哈希入库，明文只存在于本文件常量中
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            db.Users.Add(User.Create(userName, passwordHash, roleId));
            added = true;
        }

        if (added) await db.SaveChangesAsync();
    }
}
