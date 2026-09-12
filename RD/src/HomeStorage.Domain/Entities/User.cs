namespace HomeStorage.Domain.Entities;

/// <summary>
/// 用户领域实体
/// </summary>
public class User
{
    public int Id { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }

    //外键
    public int RoleId { get; private set; }
    public Role Role {get; private set;} = null!;

    private User() { }

    public static User Create(string userName, string password, int roleId, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("用户名不能为空", nameof(userName));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("密码不能为空", nameof(password));

        if (roleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roleId), "角色分类 ID 必须大于 0");

        return new User
        {
            UserName = userName.Trim(),
            Password = password, // 实际业务中存储 Hash 校验值
            RoleId = roleId,
            ImageUrl = imageUrl
        };
    }

    public void UpdateUserInfo(string userName, int roleId, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("用户名不能为空", nameof(userName));

        if (roleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roleId), "角色分类 ID 必须大于 0");

        UserName = userName;
        RoleId = roleId;

        if (!string.IsNullOrEmpty(imageUrl))
        {
            ImageUrl = imageUrl;
        }
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("密码不能为空", nameof(newPasswordHash));

        Password = newPasswordHash;
    }
}