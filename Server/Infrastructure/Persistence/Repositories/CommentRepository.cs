namespace Persistence.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Domain.Entities;
    using Domain.Interfaces;

    using Persistence.Context;
    using Persistence.Repositories.Interfaces;

    using Shared;

    public class CommentRepository : RepositoryBase<Comment, string>, ICommentRepository
    {
        public CommentRepository(
            ApplicationDbContext context,
            IDomainEventDispatcher dispatcher,
            ILogger<CommentRepository> logger)
            : base(context, dispatcher, logger)
        {
        }
        public async Task<Comment?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await Query()
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<Comment> AddCommentAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(comment, cancellationToken);
            await SaveChangesAsync(cancellationToken);
            return comment;
        }

        public async Task<PaginatedResult<Comment>> GetMovieCommentsAsync(
            int movieId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var query = Query()
                .Include(c => c.User)
                .Where(c => c.MovieId == movieId)
                .OrderByDescending(c => c.CreatedDate);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return PaginatedResult<Comment>.Create(items, totalCount, page, pageSize);
        }

        public async Task UpdateCommentAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            DbSet.Update(comment);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default)
        {
            var comment = await DbSet.FindAsync(new object[] { commentId }, cancellationToken);
            if (comment != null)
            {
                DbSet.Remove(comment);
                await SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<PaginatedResult<Comment>> GetUserCommentsAsync(
            string userId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var query = Query()
                .Include(c => c.Movie)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedDate);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return PaginatedResult<Comment>.Create(items, totalCount, page, pageSize);
        }
    }
}