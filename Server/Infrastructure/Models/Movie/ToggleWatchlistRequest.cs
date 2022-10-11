namespace Models.Movie
{
    using Domain.Enums;

    public class ToggleWatchlistRequest
    {
        public MediaType MediaType { get; init; }
        public int Id { get; init; }
        public string? Title { get; init; }
    }
}