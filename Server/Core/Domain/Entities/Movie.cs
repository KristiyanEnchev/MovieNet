namespace Domain.Entities
{
    public class Movie : BaseAuditableEntity
    {
        public int TmdbId { get; private set; }
        public string? Title { get; private set; }

        public decimal? VoteAverage { get; set; }
        public string? PosterPath { get; set; }
        public string? ReleaseDate { get; set; }
        public virtual ICollection<Genre> Genres { get; set; }


        public int LikeCount { get; private set; }
        public int DislikeCount { get; private set; }

        private readonly List<Comment> _comments = new();
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        private readonly List<UserMovieInteraction> _userInteractions = new();
        public IReadOnlyCollection<UserMovieInteraction> UserInteractions => _userInteractions.AsReadOnly();

        public Movie(int tmdbId, string? title, decimal? voteAverage, string?posterPath, string? releaseDate)
        {
            Id = Guid.NewGuid().ToString();
            TmdbId = tmdbId;
            Title = title;
            VoteAverage = voteAverage;
            PosterPath = posterPath;
            ReleaseDate = releaseDate;
            Genres = new HashSet<Genre>();
        }

        public void AddLike() => LikeCount++;
        public void RemoveLike() => LikeCount--;
        public void AddDislike() => DislikeCount++;
        public void RemoveDislike() => DislikeCount--;
        public void UpdateTmdbData(int tmdbId, string title)
        {
            Title = title;
            TmdbId = tmdbId;
        }
    }
}