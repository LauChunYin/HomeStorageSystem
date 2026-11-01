namespace HomeStorage.Application.Interfaces;

public interface IUserScopeProvider
{
  Task<UserScope> GetScopeAsync();
}

public class UserScope
{
  public bool IsAdmin { get; init; }
  public IReadOnlyList<int> CategoryIds { get; init; } = Array.Empty<int>();
  public IReadOnlyList<int> LocationIds { get; init; } = Array.Empty<int>();
}