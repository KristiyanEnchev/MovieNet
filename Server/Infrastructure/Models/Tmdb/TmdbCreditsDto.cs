namespace Models.Tmdb
{
    public class TmdbCreditsDto
    {
        public int Id { get; set; }
        public List<TmdbCastDto> Cast { get; set; }
    }
}