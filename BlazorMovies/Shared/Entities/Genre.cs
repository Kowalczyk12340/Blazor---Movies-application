using System.ComponentModel.DataAnnotations;
using BlazorMovies.Shared.Resources;

namespace BlazorMovies.Shared.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
        [Required(
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = nameof(Resource.required))]
        public string Name { get; set; }
    }
}
