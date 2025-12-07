namespace YourFilms.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string MediaType { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }

        public string ReleaseDate { get; set; }
        public int Runtime { get; set; }

        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }

        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public double Popularity { get; set; }

        public List<GenreDTO> Genres { get; set; }
    }
}
