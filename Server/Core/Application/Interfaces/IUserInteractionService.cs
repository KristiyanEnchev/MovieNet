namespace Application.Interfaces
{
    using Domain.Enums;

    using Models.Comments;
    using Models.Movie;

    using Shared;

    public interface IUserInteractionService
    {
        Task<Result<bool>> ToggleLikeAsync(
            MediaType mediaType,
            string userId,
            int movieId,
            string title,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> ToggleDislikeAsync(
            MediaType mediaType,
            string userId,
            int movieId,
            string title,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> ToggleWatchlistAsync(
            MediaType mediaType,
            string userId,
            int movieId,
            string title,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<MovieDto>>> GetUserWatchlistAsync(
            string userId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        Task<Result<UserMovieInteractionDto>> GetUserInteractionAsync(
            MediaType mediaType,
            string userId,
            int movieId,
            string title,
            CancellationToken cancellationToken = default);

        Task<Result<CommentDto>> AddCommentAsync(
            string userId,
            int movieId,
            string content,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<CommentDto>>> GetMovieCommentsAsync(
            int movieId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        Task<Result<string>> DeleteCommentAsync(
            string commentId,
            int movieId,
            string userId,
            CancellationToken cancellationToken = default);
    }
}