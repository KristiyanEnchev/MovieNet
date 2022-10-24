namespace Persistence.Repositories
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    using Domain.Entities;
    using Domain.Interfaces;

    using Persistence.Context;
    using Persistence.Repositories.Interfaces;

    public abstract class RepositoryBase<TEntity, TId> : IRepositoryBase<TEntity, TId>
        where TEntity : BaseAuditableEntity
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<TEntity> DbSet;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ILogger<RepositoryBase<TEntity, TId>> _logger;

        protected RepositoryBase(
            ApplicationDbContext context,
            IDomainEventDispatcher dispatcher,
            ILogger<RepositoryBase<TEntity, TId>> logger)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public async Task Dispatch(ICollection<TEntity> entity)
        {
            await _dispatcher.DispatchAndClearEvents(entity);
        }

        public IExecutionStrategy GetExecutionStrategy()
        {
            return Context.Database.CreateExecutionStrategy();
        }

        public virtual IQueryable<TEntity> Query(bool asNoTracking = true)
        {
            var query = DbSet.AsQueryable();
            return asNoTracking ? query.AsNoTracking() : query;
        }

        public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await DbSet.FindAsync(new object[] { id }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity of type {EntityType} with ID {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await Query().ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all entities of type {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await DbSet.AddAsync(entity, cancellationToken);
                await SaveChangesAsync(cancellationToken);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                DbSet.Remove(entity);
                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity of type {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Context.SaveChangesAsync(cancellationToken);
                await _dispatcher.DispatchAndClearEvents(GetEntitiesWithEvents());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes for entity type {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(e => EF.Property<TId>(e, "Id").Equals(id), cancellationToken);
        }

        protected virtual IEnumerable<BaseAuditableEntity> GetEntitiesWithEvents()
        {
            return Context.ChangeTracker
                .Entries<BaseAuditableEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();
        }

        protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
        {
            return SpecificationEvaluator<TEntity>.GetQuery(Query(), spec);
        }
    }
}
