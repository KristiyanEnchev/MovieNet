namespace Models.Tmdb
{
    using System.Text.Json.Serialization;

    public class TmdbVideoResultsDto
    {
        [JsonPropertyName("results")]
        public List<TmdbVideoDto> Results { get; set; } = new();
    }
}