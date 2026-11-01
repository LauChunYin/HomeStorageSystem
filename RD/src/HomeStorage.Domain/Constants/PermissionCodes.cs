namespace HomeStorage.Domain.Constants;

/// <summary>
/// 系统功能权限点编码常量。
/// <para>
/// 一个编码同时承担三个角色，务必保持三者一致：
/// 1. 数据库 Permission.PermissionCode 的取值（角色授权时写入 RolePermission）；
/// 2. JWT 中 "Permission" Claim 的值（登录时由 AuthController 写入）；
/// 3. ASP.NET Core 授权策略名（见 Api 层 Program.cs 的 AddPolicy 注册）。
/// </para>
/// </summary>
public static class PermissionCodes
{
    // ---------- 物料 ----------
    public const string ItemCreate = "item.create";
    public const string ItemUpdate = "item.update";
    public const string ItemDelete = "item.delete";

    // ---------- 品类 ----------
    public const string CategoryCreate = "category.create";
    public const string CategoryUpdate = "category.update";
    public const string CategoryDelete = "category.delete";

    // ---------- 位置 ----------
    public const string LocationCreate = "location.create";
    public const string LocationUpdate = "location.update";
    public const string LocationDelete = "location.delete";

    // ---------- 用户 ----------
    public const string UserCreate = "user.create";
    public const string UserUpdate = "user.update";
    public const string UserDelete = "user.delete";

    // ---------- 角色授权（数据范围 + 功能权限的分配） ----------
    public const string RolePermissionAssign = "role.permission.assign";
    public const string RoleCategoryAssign = "role.category.assign";
    public const string RoleLocationAssign = "role.location.assign";

    /// <summary>
    /// 全部权限点。用于批量注册授权策略，也可作为初始化 Permission 表的种子数据。
    /// </summary>
    public static readonly IReadOnlyList<string> All = new[]
    {
        ItemCreate, ItemUpdate, ItemDelete,
        CategoryCreate, CategoryUpdate, CategoryDelete,
        LocationCreate, LocationUpdate, LocationDelete,
        UserCreate, UserUpdate, UserDelete,
        RolePermissionAssign, RoleCategoryAssign, RoleLocationAssign
    };
}
