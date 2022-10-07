namespace Models.Tmdb
{
    using System.Text.Json.Serialization;

    public class TmdbGenreDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}