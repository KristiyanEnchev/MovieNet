namespace Persistence.Repositories.Interfaces
{
    using Microsoft.EntityFrameworkCore.Storage;

    using Domain.Entities;

    using Shared;

    public interface IMovieRepository : IRepositoryBase<Movie, string>
    {
        Task<PaginatedResult<Movie>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
        Task<Movie> GetByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default);
        Task<List<Movie>> GetMoviesForRatingUpdateAsync(int hoursThreshold = 24, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default);
        IExecutionStrategy GetExecutionStrategy();
    }
}