using Microsoft.AspNetCore.Authorization;

namespace HomeStorage.Api.Authorization;

/// <summary>
/// 表示「当前调用者必须拥有指定功能权限点」这一授权要求。
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
            throw new ArgumentException("权限点编码不能为空", nameof(permissionCode));

        PermissionCode = permissionCode;
    }

    /// <summary>需要校验的权限点编码，例如 item.create。</summary>
    public string PermissionCode { get; }
}
