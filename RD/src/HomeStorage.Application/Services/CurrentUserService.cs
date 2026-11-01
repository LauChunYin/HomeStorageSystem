using System.Security.Claims;
using HomeStorage.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HomeStorage.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public int UserId =>
        int.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

    public int RoleId =>
        int.TryParse(User?.FindFirst("RoleId")?.Value, out var id) ? id : 0;

    public string RoleCode =>
        User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin =>
        string.Equals(RoleCode, "Admin", StringComparison.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> Permissions =>
        User?.FindAll("Permission").Select(c => c.Value).ToArray()
        ?? Array.Empty<string>();
}