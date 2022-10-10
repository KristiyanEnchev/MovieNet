namespace Models
{
    public class TmdbOptions
    {
        public const string Section = "Tmdb";

        public string BaseUrl { get; set; } = "https://api.themoviedb.org/3";
        public string ApiKey { get; set; }
        public string ImageBaseUrl { get; set; } = "https://image.tmdb.org/t/p/";
        public int RequestsPerSecondLimit { get; set; } = 4;
    }
}