namespace HomeStorage.Application.Interfaces;

public interface ICurrentUserService
{
  int UserId { get; }
  int RoleId { get; }
  string RoleCode { get; }
  bool IsAuthenticated { get; }
  bool IsAdmin { get; }
  IReadOnlyCollection<string> Permissions { get; }
}