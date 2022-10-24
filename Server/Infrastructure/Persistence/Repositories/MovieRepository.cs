namespace Persistence.Repositories
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    using Domain.Entities;
    using Domain.Interfaces;

    using Persistence.Context;
    using Persistence.Repositories.Interfaces;

    using Shared;

    public class MovieRepository : RepositoryBase<Movie, string>, IMovieRepository
    {
        public MovieRepository(
            ApplicationDbContext context,
            IDomainEventDispatcher dispatcher,
            ILogger<MovieRepository> logger)
            : base(context, dispatcher, logger)
        {
        }

        public new IExecutionStrategy GetExecutionStrategy()
        {
            return base.GetExecutionStrategy();
        }

        public async Task<PaginatedResult<Movie>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var query = Query()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(m =>
                    m.Title.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return PaginatedResult<Movie>.Create(items, totalCount, pageNumber, pageSize);
        }

        public override async Task<Movie?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await Query()
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<Movie?> GetByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default)
        {
            return await Query()
                    .FirstOrDefaultAsync(m => m.TmdbId == tmdbId, cancellationToken);
        }

        public async Task<bool> ExistsByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(m => m.TmdbId == tmdbId, cancellationToken);
        }

        public async Task<List<Movie>> GetMoviesForRatingUpdateAsync(
            int hoursThreshold = 24,
            CancellationToken cancellationToken = default)
        {
            var thresholdDate = DateTime.UtcNow.AddHours(-hoursThreshold);

            return await DbSet
                .Where(m => m.UpdatedDate == null || m.UpdatedDate < thresholdDate)
                .OrderBy(m => m.UpdatedDate)
                .Take(100) // Limit batch size
                .ToListAsync(cancellationToken);
        }
    }
}