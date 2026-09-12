using HomeStorage.Application.Dtos;

namespace HomeStorage.Application.Interfaces;

public interface IUserService
{
    // 获取所有用户信息
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();

    // 根据 用户名 查询单个用户信息
    Task<UserResponseDto?> GetUserByUserNameAsync(string userName);

    // 创建新用户
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);

    // 更新用户信息（如果用户不存在返回 null）
    Task<UserResponseDto?> UpdateUserInfoAsync(int id, UpdateUserInfoDto dto);

    // 修改用户密码（如果用户不存在返回 null）
    Task ChangePasswordAsync(int id, ChangePasswordDto dto);

    // 删除用户（成功返回 true，不存在返回 false）
    Task<bool> DeleteUserAsync(int id);
}