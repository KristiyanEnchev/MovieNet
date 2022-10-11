namespace Models.Movie
{
    public class CastDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Character { get; set; }
        public string ProfilePath { get; set; }
        public int Order { get; set; }
        public string Department { get; set; }
        public decimal Popularity { get; set; }
    }
}