namespace Models.Tmdb
{
    using System.Text.Json.Serialization;

    public class TmdbMovieDetailsDto : TmdbMovieDto
    {
        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }

        [JsonPropertyName("tagline")]
        public string Tagline { get; set; }

        [JsonPropertyName("genres")]
        public List<TmdbGenreDto> Genres { get; set; } = new();

        public List<TmdbCastDto> Cast { get; set; } = new();
        public TmdbVideoResultsDto? Videos { get; set; }

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; }

        [JsonPropertyName("spoken_languages")]
        public List<SpokenLanguageDto> SpokenLanguages { get; set; } = new();

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("budget")]
        public decimal Budget { get; set; }

        [JsonPropertyName("revenue")]
        public decimal Revenue { get; set; }
    }

    public class SpokenLanguageDto
    {
        [JsonPropertyName("iso_639_1")]
        public string Iso6391 { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}