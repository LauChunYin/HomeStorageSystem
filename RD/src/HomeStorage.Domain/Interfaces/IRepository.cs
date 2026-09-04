using System.Linq.Expressions;

namespace HomeStorage.Domain.Interfaces;

/// <summary>
/// 通用仓储接口：定义泛型基础 CRUD 操作
/// </summary>
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}