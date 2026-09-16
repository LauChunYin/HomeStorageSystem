using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByNameAsync(string userName);
    Task<bool> SaveChangesAsync();
}