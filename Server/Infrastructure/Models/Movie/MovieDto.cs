namespace Models.Movie
{
    using Domain.Enums;

    public class MovieDto
    {
        public string Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
        public MediaType MediaType { get; set; }
        public string ReleaseDate { get; set; }
        public decimal? VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public decimal Popularity { get; set; }

        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public bool IsWatchlisted { get; set; }
    }
}