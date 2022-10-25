namespace Persistence.Repositories.Interfaces
{
    using Shared;

    using Domain.Entities;

    public interface ICommentRepository
    {
        Task<Comment> AddCommentAsync(Comment comment, CancellationToken cancellationToken = default);
        Task<Comment?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PaginatedResult<Comment>> GetUserCommentsAsync(
            string userId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        Task UpdateCommentAsync(Comment comment, CancellationToken cancellationToken = default);
        Task DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default);

        Task<PaginatedResult<Comment>> GetMovieCommentsAsync(
            int movieId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
    }
}
