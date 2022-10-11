namespace Models.Movie
{
    using Domain.Entities;
    using Domain.Enums;

    using Models.Comments;

    using Shared.Mappings;

    public class UserMovieInteractionDto : IMapFrom<UserMovieInteraction>
    {
        public int MovieId { get; set; }
        public MediaType MediaType { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public bool IsWatchlisted { get; set; }
        public List<CommentDto>? Comments { get; set; }
    }
}
