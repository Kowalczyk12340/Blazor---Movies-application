using BlazorMovies.Shared.Entities;

namespace BlazorMovies.Shared.DTOs
{
    public class MovieUpdateDTO
    {
        public Movie Movie { get; set; }
        public List<Person> Actors { get; set; }
        public List<Genre> SelectedGenres { get; set; }
        public List<Genre> NotSelectedGenres { get; set; }
    }
}
