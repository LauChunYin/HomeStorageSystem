using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class UserService : IUserService
{
    // 注入我们第三步写好的物品仓储管家
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    // 获取所有用户信息
    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        //返回所有用户信息
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToResponseDto);
    }

    // 根据 用户名 查询单个用户信息
    public async Task<UserResponseDto?> GetUserByUserNameAsync(string userName)
    {
        var user = await _userRepository.GetUserByName(userName);
        return user == null ? null :MapToResponseDto(user);
    }

    // 创建新用户
    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var existingUser = await _userRepository.GetUserByName(dto.UserName);
        if(existingUser is not null)
            throw new BusinessException("用户名已使用，请更换其它用户名");

        // 使用 BCrypt 加密
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = User.Create(dto.UserName, passwordHash, dto.RoleId, dto.ImageUrl);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return MapToResponseDto(user);
    }

    // 更新用户信息（如果物品不存在返回 null）
    public async Task<UserResponseDto?> UpdateUserInfoAsync(int id, UpdateUserInfoDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("用户不存在, 请检查");

        user.UpdateUserInfo(dto.UserName, dto.RoleId, dto.ImageUrl);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        return MapToResponseDto(user);
    }

    // 修改用户密码（如果物品不存在返回 null）
    public async Task ChangePasswordAsync(int id, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("用户不存在, 请检查");

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
            throw new BusinessException("原密码不正确");

        if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.Password))
            throw new BusinessException("新密码不能与旧密码相同");

        string newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ChangePassword(newHash);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    // 删除用户（成功返回 true，不存在返回 false）
    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("用户不存在, 请检查");

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
        return true;
    }

    private static UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            RoleId = user.RoleId,
            RoleName = user.Role?.RoleName ?? string.Empty,
            RoleCode = user.Role?.RoleCode ?? string.Empty,
            ImageUrl = user.ImageUrl,
            Permissions = user.Role?.RolePermissions
            .Where(rp => rp.Permission != null)
            .Select(rp => rp.Permission.PermissionCode)
            .ToList() ?? new List<string>()
        };
    }
}