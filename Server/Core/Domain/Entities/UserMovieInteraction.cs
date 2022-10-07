namespace Domain.Entities
{
    using Domain.Enums;
    using Domain.Events;

    public class UserMovieInteraction : BaseAuditableEntity
    {
        public string UserId { get; private set; }
        public User User { get; private set; }
        public int MovieId { get; private set; }
        public virtual Movie Movie { get; private set; }

        public MediaType MediaType { get; private set; }
        public bool IsLiked { get; private set; }
        public bool IsDisliked { get; private set; }
        public bool IsWatchlisted { get; private set; }

        private UserMovieInteraction() { }

        public UserMovieInteraction(string userId, int movieId, MediaType mediaType)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("UserId cannot be null or empty");

            UserId = userId;
            MovieId = movieId;
            MediaType = mediaType;
            IsLiked = false;
            IsDisliked = false;
            IsWatchlisted = false;
        }

        public void ToggleLike()
        {
            if (IsDisliked)
            {
                IsDisliked = false;
            }
            IsLiked = !IsLiked;
            AddDomainEvent(new UserMovieInteractionChangedEvent(this));
        }

        public void ToggleDislike()
        {
            if (IsLiked)
            {
                IsLiked = false;
            }
            IsDisliked = !IsDisliked;
            AddDomainEvent(new UserMovieInteractionChangedEvent(this));
        }

        public void ToggleWatchlist()
        {
            IsWatchlisted = !IsWatchlisted;
            AddDomainEvent(new UserMovieInteractionChangedEvent(this));
        }
    }
}