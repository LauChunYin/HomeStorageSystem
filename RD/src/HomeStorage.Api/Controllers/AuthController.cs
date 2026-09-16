using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeStorage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public AuthController(
        IUserRepository userRepository, 
        IConfiguration configuration,
        IUserService userService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        // 1. 查出包含 Role 和 Permission 的完整用户对象
        var user = await _userRepository.GetUserByNameAsync(dto.UserName);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            return Unauthorized("用户名或密码错误");
        }

        // 2. 组装 JWT Claims（包含身份、角色与细粒度权限点）
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("RoleId", user.RoleId.ToString()),
            new Claim(ClaimTypes.Role, user.Role?.RoleCode ?? "Guest")
        };

        // 3. 将该角色拥有的所有 PermissionCode 写入 Claim
        if (user.Role?.RolePermissions != null)
        {
            foreach (var rp in user.Role.RolePermissions)
            {
                if (rp.Permission != null)
                {
                    claims.Add(new Claim("Permission", rp.Permission.PermissionCode));
                }
            }
        }

        // 4. 签发 JWT Token
        var secretKey = _configuration["JwtSettings:Secret"] 
            ?? "YourSuperSecretKeyHereAtLeast32BytesLong!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"] ?? "HomeStorageApi",
            audience: _configuration["JwtSettings:Audience"] ?? "HomeStorageClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        // 5. 获取 mapped DTO
        var userDto = await _userService.GetUserByUserNameAsync(user.UserName);

        return Ok(new AuthResponseDto
        {
            Token = jwtToken,
            User = userDto!
        });
    }
}