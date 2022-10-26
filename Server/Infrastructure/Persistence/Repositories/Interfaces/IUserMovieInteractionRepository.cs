namespace Persistence.Repositories.Interfaces
{
    using Domain.Entities;

    using Shared;

    public interface IUserMovieInteractionRepository
    {
        Task<UserMovieInteraction> GetUserInteractionAsync(
            string userId,
            int movieId,
            CancellationToken cancellationToken = default);

        Task<List<UserMovieInteraction>> GetUserInteractionsAsync(
            string userId,
            List<int> movieIds,
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<UserMovieInteraction>> GetUserWatchlistAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<UserMovieInteraction>> GetUserLikedMoviesAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<UserMovieInteraction> AddOrUpdateAsync(
            UserMovieInteraction interaction,
            CancellationToken cancellationToken = default);

        Task<bool> RemoveAsync(
            string userId,
            int movieId,
            CancellationToken cancellationToken = default);
    }
}