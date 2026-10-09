namespace HomeStorage.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRoleRepository Roles{ get; }
    IUserRepository Users { get; }
    IItemRepository Items { get; }
    ICategoryRepository Categories { get; }
    ILocationRepository Locations { get; }


    /// <summary>
    /// 统一提交当前上下文中的所有变更并保存至数据库
    /// </summary>
    /// <returns>受影响的行数是否大于0</returns>
    Task<bool> SaveChangesAsync();

    /// <summary>
    /// 显式开启事务（用于复杂操作）
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// 提交事务
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// 回滚事务
    /// </summary>
    Task RollbackAsync();
}