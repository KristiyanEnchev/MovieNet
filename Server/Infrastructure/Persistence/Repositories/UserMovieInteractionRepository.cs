namespace Persistence.Repositories
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Domain.Entities;
    using Domain.Interfaces;

    using Persistence.Context;
    using Persistence.Repositories.Interfaces;

    using Shared;

    public class UserMovieInteractionRepository : RepositoryBase<UserMovieInteraction, string>, IUserMovieInteractionRepository
    {
        public UserMovieInteractionRepository(
            ApplicationDbContext context,
            IDomainEventDispatcher dispatcher,
            ILogger<UserMovieInteractionRepository> logger)
            : base(context, dispatcher, logger)
        {
        }

        public async Task<UserMovieInteraction?> GetUserInteractionAsync(
            string userId,
            int movieId,
            CancellationToken cancellationToken = default)
        {
            return await Query()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.MovieId == movieId,
                    cancellationToken);
        }

        public async Task<List<UserMovieInteraction>> GetUserInteractionsAsync(
            string userId,
            List<int> movieIds,
            CancellationToken cancellationToken = default)
        {
            return await Query()
                .Where(x =>
                    x.UserId == userId &&
                    movieIds.Contains(x.MovieId))
                .ToListAsync(cancellationToken);
        }

        public async Task<PaginatedResult<UserMovieInteraction>> GetUserWatchlistAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = Query()
                .Where(x => x.UserId == userId && x.IsWatchlisted)
                .Include(x => x.Movie)
                .OrderByDescending(x => x.UpdatedDate);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return PaginatedResult<UserMovieInteraction>.Create(items, totalCount, page, pageSize);
        }

        public async Task<PaginatedResult<UserMovieInteraction>> GetUserLikedMoviesAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = Query()
                .Where(x => x.UserId == userId && x.IsLiked)
                .Include(x => x.Movie)
                .OrderByDescending(x => x.UpdatedDate);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return PaginatedResult<UserMovieInteraction>.Create(items, totalCount, page, pageSize);
        }

        public async Task<UserMovieInteraction> AddOrUpdateAsync(
            UserMovieInteraction interaction,
            CancellationToken cancellationToken = default)
        {
            var existing = await GetUserInteractionAsync(
                interaction.UserId,
                interaction.MovieId,
                cancellationToken);

            if (existing == null)
            {
                return await AddAsync(interaction, cancellationToken);
            }

            DbSet.Update(interaction);
            await SaveChangesAsync(cancellationToken);
            return interaction;
        }

        public async Task<List<UserMovieInteraction>> GetUserRecentInteractionsAsync(
            string userId,
            int limit = 10,
            CancellationToken cancellationToken = default)
        {
            return await Query()
                .Where(x => x.UserId == userId)
                .Include(x => x.Movie)
                .OrderByDescending(x => x.UpdatedDate)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> RemoveAsync(
            string userId,
            int movieId,
            CancellationToken cancellationToken = default)
        {
            var interaction = await GetUserInteractionAsync(userId, movieId, cancellationToken);

            if (interaction == null)
            {
                return false;
            }

            DbSet.Remove(interaction);
            await SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}