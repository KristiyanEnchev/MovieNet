namespace Models.Tmdb
{
    using System.Text.Json.Serialization;

    using Domain.Enums;

    public class TmdbMovieDto
    {
        [JsonPropertyName("id")]
        public int TmdbId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; }

        [JsonPropertyName("original_name")]
        public string OriginalName { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("first_air_date")]
        public string FirstAirDate { get; set; }

        [JsonPropertyName("vote_average")]
        public decimal VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int> GenreIds { get; set; }

        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonPropertyName("adult")]
        public bool Adult { get; set; }

        [JsonPropertyName("popularity")]
        public decimal Popularity { get; set; }

        private MediaType _mediaType = MediaType.movie;

        [JsonPropertyName("media_type")]
        public string MediaTypeString
        {
            get => _mediaType.ToString().ToLower();
            set
            {
                if (Enum.TryParse<MediaType>(value, true, out var parsedType))
                {
                    _mediaType = parsedType;
                }
            }
        }
        public MediaType MediaType
        {
            get => _mediaType;
            set
            {
                _mediaType = value;
            }
        }

        [JsonPropertyName("origin_country")]
        public List<string> OriginCountry { get; set; }

        public string GetTitle() => !string.IsNullOrEmpty(Title) ? Title : Name;
        public string GetOriginalTitle() => !string.IsNullOrEmpty(OriginalTitle) ? OriginalTitle : OriginalName;
        public string GetReleaseDate() => !string.IsNullOrEmpty(ReleaseDate) ? ReleaseDate : FirstAirDate;
    }
}