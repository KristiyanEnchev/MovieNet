namespace Models.Movie
{
    public class MovieDetailsDto : MovieDto
    {
        public int Runtime { get; set; }
        public string Status { get; set; }
        public string Tagline { get; set; }
        public string Homepage { get; set; }
        public decimal Budget { get; set; }
        public decimal Revenue { get; set; }
        public string OriginalLanguage { get; set; }
        public bool Adult { get; set; }

        public List<GenreDto> Genres { get; set; } = new();
        public List<CastDto> Cast { get; set; } = new();
        public List<VideoDto>? Videos { get; set; } = new();
    }
}
