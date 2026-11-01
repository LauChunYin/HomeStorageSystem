using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeStorage.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public IRoleRepository Roles { get; }
    public IUserRepository Users { get; }
    public IItemRepository Items { get; }
    public ICategoryRepository Categories { get; }
    public ILocationRepository Locations { get; }

    public UnitOfWork(
        AppDbContext context,
        IRoleRepository roles,
        IUserRepository users,
        IItemRepository items,
        ICategoryRepository categories,
        ILocationRepository locations)
    {
        _context = context;
        Roles = roles;
        Users = users;
        Items = items;
        Categories = categories;
        Locations = locations;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task BeginTransactionAsync()
    {
        // 必须把事务句柄保存到字段，否则 Commit/Rollback 会因为 _transaction 为 null 而静默失效
        if (_transaction != null) return;
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction == null) return;
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null) return;
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        // 未提交的事务随上下文释放一起回滚，这里显式释放避免连接泄漏
        _transaction?.Dispose();
        _transaction = null;
        _context.Dispose();
    }
}