namespace Persistence.Repositories
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Domain.Enums;
    using Domain.Entities;
    using Domain.Interfaces;

    using Persistence.Context;
    using Persistence.Repositories.Interfaces;

    public class GenreRepository : RepositoryBase<Genre, string>, IGenreRepository
    {
        public GenreRepository(
            ApplicationDbContext context,
            IDomainEventDispatcher dispatcher,
            ILogger<GenreRepository> logger)
            : base(context, dispatcher, logger)
        {
        }

        public async Task<List<Genre>> GetGenresByTmdbIdsAsync(List<int> tmdbIds, CancellationToken cancellationToken = default)
        {
            return await Query()
                .Where(g => tmdbIds.Contains(g.TmdbId))
                .ToListAsync(cancellationToken);
        }

        public async Task<Genre?> GetGenreByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default)
        {
            return await Query()
                .FirstOrDefaultAsync(g => g.TmdbId == tmdbId, cancellationToken);
        }

        public async Task<List<Genre>> GetAllGenresAsync(MediaType mediaType, CancellationToken cancellationToken = default)
        {
            return await Query()
                .Where(g => g.MediaType == mediaType)
                .OrderBy(g => g.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task AddGenreAsync(Genre genre, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(genre, cancellationToken);
        }

        public async Task AddGenresAsync(List<Genre> genres, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(genres, cancellationToken);
        }

        public async Task UpdateGenreAsync(Genre genre, CancellationToken cancellationToken = default)
        {
            DbSet.Update(genre);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateGenresAsync(List<Genre> genres, CancellationToken cancellationToken = default)
        {
            DbSet.UpdateRange(genres);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Genre>> GetOrAddGenresAsync(
            List<(int TmdbId, string Name, MediaType MediaType)> genreData,
            CancellationToken cancellationToken = default)
        {
            var existingGenres = await GetGenresByTmdbIdsAsync(
                genreData.Select(g => g.TmdbId).ToList(),
                cancellationToken);

            var newGenres = new List<Genre>();
            var genresToUpdate = new List<Genre>();

            foreach (var (tmdbId, name, mediaType) in genreData)
            {
                var genre = existingGenres.FirstOrDefault(g => g.TmdbId == tmdbId);
                if (genre == null)
                {
                    genre = new Genre(tmdbId, name, mediaType);
                    newGenres.Add(genre);
                }
                else if (genre.Name != name)
                {
                    genre.UpdateName(name);
                    genresToUpdate.Add(genre);
                }
            }

            if (newGenres.Any())
            {
                await AddGenresAsync(newGenres, cancellationToken);
            }

            if (genresToUpdate.Any())
            {
                await UpdateGenresAsync(genresToUpdate, cancellationToken);
            }

            await SaveChangesAsync(cancellationToken);

            return existingGenres.Concat(newGenres).ToList();
        }
    }
}