namespace Infrastructure.Services.Movie
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;

    using AutoMapper;

    using Application.Interfaces;

    using Domain.Enums;
    using Domain.Events;
    using Domain.Entities;

    using Models.Movie;
    using Models.Comments;

    using Persistence.Repositories.Interfaces;

    using Shared;
    using Shared.Interfaces;

    public class UserInteractionService : IUserInteractionService
    {
        private readonly IUserMovieInteractionRepository _interactionRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ITmdbService _tmdbService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserInteractionService> _logger;
        private readonly ITransactionHelper _transactionHelper;
        private readonly UserManager<User> userManager;

        public UserInteractionService(
            IUserMovieInteractionRepository interactionRepository,
            IMovieRepository movieRepository,
            IMapper mapper,
            ILogger<UserInteractionService> logger,
            ICommentRepository commentRepository,
            ITmdbService tmdbService,
            IGenreRepository genreRepository,
            ITransactionHelper transactionHelper,
            UserManager<User> userManager)
        {
            _interactionRepository = interactionRepository;
            _movieRepository = movieRepository;
            _mapper = mapper;
            _logger = logger;
            _commentRepository = commentRepository;
            _tmdbService = tmdbService;
            _genreRepository = genreRepository;
            _transactionHelper = transactionHelper;
            this.userManager = userManager;
        }

        private async Task<Movie> EnsureMovieExistsAsync(MediaType mediaType, int tmdbId, string title, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetByTmdbIdAsync(tmdbId, cancellationToken);
            if (movie == null)
            {
                var details = await _tmdbService.GetDetailsAsync(mediaType, tmdbId, false, cancellationToken);
                if (!details.Success)
                {
                    throw new Exception($"Failed to get movie details from TMDB: {string.Join(", ", details.Errors)}");
                }

                var strategy = _movieRepository.GetExecutionStrategy();

                await strategy.ExecuteAsync(async (ct) =>
                {
                    using var transaction = await _transactionHelper.BeginTransactionAsync();
                    try
                    {
                        movie = new Movie(tmdbId, details.Data.Title, details.Data.VoteAverage, details.Data.PosterPath, details.Data.ReleaseDate);

                        if (details.Data.Genres != null && details.Data.Genres.Any())
                        {
                            var genreIds = details.Data.Genres.Select(g => g.Id).ToList();
                            var existingGenres = await _genreRepository.GetGenresByTmdbIdsAsync(genreIds, ct);

                            foreach (var genreDto in details.Data.Genres)
                            {
                                var genre = existingGenres.FirstOrDefault(g => g.TmdbId == genreDto.Id);
                                if (genre == null)
                                {
                                    genre = new Genre(genreDto.Id, genreDto.Name, mediaType);
                                    await _genreRepository.AddGenreAsync(genre, ct);
                                    await _genreRepository.SaveChangesAsync(ct);
                                }
                            }
                        }

                        await _movieRepository.AddAsync(movie, ct);
                        await _movieRepository.SaveChangesAsync(ct);

                        await transaction.CommitAsync(ct);
                        return movie;
                    }
                    catch(Exception ex)
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                }, cancellationToken);
            }
            return movie;
        }

        public async Task<Result<bool>> ToggleLikeAsync(
             MediaType mediaType,
             string userId,
             int movieId,
             string title,
             CancellationToken cancellationToken = default)
        {
            try
            {
                var movie = await EnsureMovieExistsAsync(mediaType, movieId, title, cancellationToken);

                var interaction = await _interactionRepository.GetUserInteractionAsync(userId, movie.TmdbId, cancellationToken)
                    ?? new UserMovieInteraction(userId, movie.TmdbId, mediaType);

                var wasLiked = interaction.IsLiked;
                interaction.ToggleLike();

                if (wasLiked)
                {
                    movie.RemoveLike();
                    movie.AddDislike();
                }
                else
                {
                    movie.AddLike();
                    movie.RemoveDislike();
                }

                await _interactionRepository.AddOrUpdateAsync(interaction, cancellationToken);
                await _movieRepository.UpdateAsync(movie, cancellationToken);

                return Result<bool>.SuccessResult(interaction.IsLiked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling like. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                return Result<bool>.Failure($"Error toggling like: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ToggleDislikeAsync(
            MediaType mediaType,
            string userId,
            int movieId,
            string title,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var movie = await EnsureMovieExistsAsync(mediaType, movieId, title, cancellationToken);

                var interaction = await _interactionRepository.GetUserInteractionAsync(userId, movie.TmdbId, cancellationToken)
                    ?? new UserMovieInteraction(userId, movie.TmdbId, mediaType);

                var wasDisliked = interaction.IsDisliked;
                interaction.ToggleDislike();

                if (wasDisliked) movie.RemoveDislike();
                else movie.AddDislike();

                await _interactionRepository.AddOrUpdateAsync(interaction, cancellationToken);
                await _movieRepository.UpdateAsync(movie, cancellationToken);

                return Result<bool>.SuccessResult(interaction.IsDisliked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling dislike. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                return Result<bool>.Failure($"Error toggling dislike: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ToggleWatchlistAsync(
             MediaType mediaType,
             string userId,
             int movieId,
             string title,
             CancellationToken cancellationToken = default)
        {
            try
            {
                var userExists = await ExistsAsync(userId, cancellationToken);
                if (!userExists)
                {
                    throw new UnauthorizedAccessException("Not a valid user");
                }

                var movie = await EnsureMovieExistsAsync(mediaType, movieId, title, cancellationToken);

                var interaction = await _interactionRepository.GetUserInteractionAsync(userId, movie.TmdbId, cancellationToken)
                    ?? new UserMovieInteraction(userId, movie.TmdbId, mediaType);

                interaction.ToggleWatchlist();
                await _interactionRepository.AddOrUpdateAsync(interaction, cancellationToken);

                return Result<bool>.SuccessResult(interaction.IsWatchlisted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling watchlist. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                return Result<bool>.Failure($"Error toggling watchlist: {ex.Message}");
            }
        }

        public async Task<Result<PaginatedResult<MovieDto>>> GetUserWatchlistAsync(
            string userId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _interactionRepository.GetUserWatchlistAsync(userId, page, pageSize, cancellationToken);
                var movies = result.Data.Select(i => {
                    var movieDto = _mapper.Map<MovieDto>(i.Movie);
                    movieDto.PosterPath = i.Movie.PosterPath;
                    movieDto.VoteAverage = i.Movie.VoteAverage;
                    movieDto.ReleaseDate = i.Movie.ReleaseDate;
                    movieDto.MediaType = i.MediaType;
                    movieDto.IsLiked = i.IsLiked;
                    movieDto.IsDisliked = i.IsDisliked;
                    movieDto.IsWatchlisted = i.IsWatchlisted;
                    return movieDto;
                }).ToList();

                return Result<PaginatedResult<MovieDto>>.SuccessResult(
                    PaginatedResult<MovieDto>.Create(movies, result.TotalCount, page, pageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user watchlist. UserId: {UserId}", userId);
                return Result<PaginatedResult<MovieDto>>.Failure($"Error getting user watchlist: {ex.Message}");
            }
        }

        public async Task<Result<UserMovieInteractionDto>> GetUserInteractionAsync(
            MediaType mediaType,
            string userId,
            int movieId,
            string title,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureMovieExistsAsync(mediaType, movieId, title, cancellationToken);

                var interaction = await _interactionRepository.GetUserInteractionAsync(userId, movieId, cancellationToken);
                return interaction == null
                    ? Result<UserMovieInteractionDto>.SuccessResult(new UserMovieInteractionDto { MovieId = movieId, MediaType = mediaType })
                    : Result<UserMovieInteractionDto>.SuccessResult(_mapper.Map<UserMovieInteractionDto>(interaction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user interaction. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                return Result<UserMovieInteractionDto>.Failure($"Error getting user interaction: {ex.Message}");
            }
        }

        public async Task<Result<CommentDto>> AddCommentAsync(
           string userId,
           int movieId,
           string content,
           CancellationToken cancellationToken = default)
        {
            try
            {
                var comment = new Comment(content, userId, movieId);
                comment.AddDomainEvent(new CommentAddedEvent
                {
                    CommentId = comment.Id,
                    MovieId = movieId.ToString(),
                    UserId = userId,
                    Created = DateTime.UtcNow
                });

                await _commentRepository.AddCommentAsync(comment, cancellationToken);

                return Result<CommentDto>.SuccessResult(_mapper.Map<CommentDto>(comment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                return Result<CommentDto>.Failure($"Error adding comment: {ex.Message}");
            }
        }

        public async Task<Result<string>> DeleteCommentAsync(
            string commentId,
            int movieId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);

                if (comment == null)
                    return Result<string>.Failure("Comment not found");

                if (comment.UserId != userId)
                    return Result<string>.Failure("You can only delete your own comments");

                comment.AddDomainEvent(new CommentDeletedEvent
                {
                    CommentId = commentId,
                    MovieId = movieId.ToString(),
                    UserId = userId
                });

                await _commentRepository.DeleteCommentAsync(commentId, cancellationToken);

                return Result<string>.SuccessResult("Comment deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment. CommentId: {CommentId}, UserId: {UserId}", commentId, userId);
                return Result<string>.Failure($"Error deleting comment: {ex.Message}");
            }
        }


        public async Task<Result<PaginatedResult<CommentDto>>> GetMovieCommentsAsync(
            int movieId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _commentRepository.GetMovieCommentsAsync(movieId, page, pageSize, cancellationToken);
                var comments = result.Data.Select(c => _mapper.Map<CommentDto>(c)).ToList();

                return Result<PaginatedResult<CommentDto>>.SuccessResult(
                    PaginatedResult<CommentDto>.Create(comments, result.TotalCount, page, pageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie comments. MovieId: {MovieId}", movieId);
                return Result<PaginatedResult<CommentDto>>.Failure($"Error getting movie comments: {ex.Message}");
            }
        }

       
    }
}