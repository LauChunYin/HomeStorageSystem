using HomeStorage.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace HomeStorage.Api.Authorization;

/// <summary>
/// 权限点校验处理器。
/// <para>
/// 判定规则（业务决策 5）：系统管理员天然拥有全量权限，直接放行；
/// 其余角色必须在其 JWT 的 "Permission" Claim 中携带对应编码才能通过。
/// </para>
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>JWT 中承载权限点编码的 Claim 类型，需与 AuthController 签发时保持一致。</summary>
    public const string PermissionClaimType = "Permission";

    /// <summary>系统管理员角色编码。</summary>
    public const string AdminRoleCode = "Admin";

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 管理员跳过功能权限校验
        if (context.User.IsInRole(AdminRoleCode))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var hasPermission = context.User
            .FindAll(PermissionClaimType)
            .Any(claim => string.Equals(
                claim.Value,
                requirement.PermissionCode,
                StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
