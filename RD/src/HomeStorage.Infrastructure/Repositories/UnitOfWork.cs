using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;

namespace HomeStorage.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRoleRepository Roles{ get; }
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
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public void Dispose(){}
}