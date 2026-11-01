using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class UserScopeProvider : IUserScopeProvider
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUser;
  private UserScope? _cached;

  public UserScopeProvider(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
  {
    _unitOfWork = unitOfWork;
    _currentUser = currentUser;
  }

  public async Task<UserScope> GetScopeAsync()
  {
    if (_cached != null) return _cached;

    if (_currentUser.IsAdmin)
    {
      _cached = new UserScope { IsAdmin = true };
      return _cached;
    }

    var categoryIds = await _unitOfWork.Roles.GetAllowedCategoryIdsAsync(_currentUser.RoleId);
    var locationIds = await _unitOfWork.Roles.GetAllowedLocationIdsAsync(_currentUser.RoleId);

    _cached = new UserScope
    {
      IsAdmin = false,
      CategoryIds = categoryIds,
      LocationIds = locationIds
    };
    return _cached;
  }
}