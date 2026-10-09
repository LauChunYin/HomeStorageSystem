using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class UserService : IUserService
{
    // 注入我们第三步写好的物品仓储管家
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // 获取所有用户信息
    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        //返回所有用户信息
        var users = await _unitOfWork.Users.GetAllAsync();
        return users.Select(MapToResponseDto);
    }

    // 根据 用户名 查询单个用户信息
    public async Task<UserResponseDto?> GetUserByUserNameAsync(string userName)
    {
        var user = await _unitOfWork.Users.GetUserByNameAsync(userName);
        return user == null ? null :MapToResponseDto(user);
    }

    // 创建新用户
    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var existingUser = await _unitOfWork.Users.GetUserByNameAsync(dto.UserName);
        if(existingUser is not null)
            throw new BusinessException("用户名已使用，请更换其它用户名");

        // 使用 BCrypt 加密
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = User.Create(dto.UserName, passwordHash, dto.RoleId, dto.ImageUrl);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponseDto(user);
    }

    // 更新用户信息（如果物品不存在返回 null）
    public async Task<UserResponseDto?> UpdateUserInfoAsync(int id, UpdateUserInfoDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("用户不存在, 请检查");

        user.UpdateUserInfo(dto.UserName, dto.ImageUrl);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponseDto(user);
    }

    public async Task<UserResponseDto?> UpdateUserRoleAsync(int id, UpdateUserRoleDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("用户不存在, 请检查");

        user.UpdateUserRole(dto.RoleId);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponseDto(user);
    }

    // 修改用户密码（如果物品不存在返回 null）
    public async Task ChangePasswordAsync(int id, ChangePasswordDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("用户不存在, 请检查");

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
            throw new BusinessException("原密码不正确");

        if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.Password))
            throw new BusinessException("新密码不能与旧密码相同");

        string newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ChangePassword(newHash);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    // 删除用户（成功返回 true，不存在返回 false）
    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("用户不存在, 请检查");

        _unitOfWork.Users.Delete(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRolePermissionsAsync(int id, List<int> permissionIds)
    {
        var role = await _unitOfWork.Roles.GetRolePermissionsAsyns(id)
            ?? throw new KeyNotFoundException("角色不存在，请检查");

        _unitOfWork.Roles.UpdateRolePermissionsAsyns(role, permissionIds);
        
        // ④ 提交保存落盘
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRoleCategoriesAsync(int id, List<int> categoryIds)
    {
        var role = await _unitOfWork.Roles.GetRoleCategoriesAsyns(id)
            ?? throw new KeyNotFoundException("角色不存在，请检查");

        _unitOfWork.Roles.UpdateRoleCategoriesAsyns(role, categoryIds);
        
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UpdateRoleLocationsAsync(int id, List<int> locationIds)
    {
        var role = await _unitOfWork.Roles.GetRoleLocationsAsyns(id)
            ?? throw new KeyNotFoundException("角色不存在，请检查");

        _unitOfWork.Roles.UpdateRoleLocationsAsyns(role, locationIds);
        
        await _unitOfWork.SaveChangesAsync();
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