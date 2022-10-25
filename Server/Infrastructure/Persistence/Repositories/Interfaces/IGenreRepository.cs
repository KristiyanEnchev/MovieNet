namespace Persistence.Repositories.Interfaces
{
    using Domain.Entities;
    using Domain.Enums;

    public interface IGenreRepository : IRepositoryBase<Genre, string>
    {
        Task<List<Genre>> GetGenresByTmdbIdsAsync(List<int> tmdbIds, CancellationToken cancellationToken = default);
        Task<Genre?> GetGenreByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default);
        Task<List<Genre>> GetAllGenresAsync(MediaType mediaType, CancellationToken cancellationToken = default);
        Task AddGenreAsync(Genre genre, CancellationToken cancellationToken = default);
        Task AddGenresAsync(List<Genre> genres, CancellationToken cancellationToken = default);
        Task UpdateGenreAsync(Genre genre, CancellationToken cancellationToken = default);
        Task UpdateGenresAsync(List<Genre> genres, CancellationToken cancellationToken = default);
        Task<List<Genre>> GetOrAddGenresAsync(
            List<(int TmdbId, string Name, MediaType MediaType)> genreData,
            CancellationToken cancellationToken = default);
    }
}