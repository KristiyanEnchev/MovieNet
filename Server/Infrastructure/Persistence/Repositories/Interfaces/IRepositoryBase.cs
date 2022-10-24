namespace Persistence.Repositories.Interfaces
{
    using Domain.Entities;

    public interface IRepositoryBase<TEntity, TId> where TEntity : BaseEntity
    {
        Task Dispatch(ICollection<TEntity> entity);
        IQueryable<TEntity> Query(bool asNoTracking = true);
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    }
}