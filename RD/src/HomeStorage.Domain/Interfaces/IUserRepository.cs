using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByName(string userName);
    Task<bool> SaveChangesAsync();
}