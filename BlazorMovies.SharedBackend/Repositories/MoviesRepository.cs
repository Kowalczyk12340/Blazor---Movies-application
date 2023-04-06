using AutoMapper;
using BlazorMovies.Shared.DTOs;
using BlazorMovies.Shared.Entities;
using BlazorMovies.Shared.Repositories;
using BlazorMovies.SharedBackend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.SharedBackend.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationStateService _authenticationStateService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        private readonly string containerName = "movies";

        public MoviesRepository(ApplicationDbContext context, IAuthenticationStateService authenticationStateService, IFileStorageService fileStorageService, IMapper mapper)
        {
            _context = context;
            _authenticationStateService = authenticationStateService;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        public async Task<int> CreateMovie(Movie movie)
        {
            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                var poster = Convert.FromBase64String(movie.Poster);
                movie.Poster = await _fileStorageService.SaveFile(poster, "jpg", containerName);
            }

            if (movie.MoviesActors is not null)
            {
                for (var i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i + 1;
                }
            }

            await _context.AddAsync(movie);
            await _context.SaveChangesAsync();

            return movie.Id;
        }

        public async Task DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Remove(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<DetailsMovieDTO> GetDetailsMovieDTO(int id)
        {
            var movie = await _context.Movies.Where(x => x.Id == id)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (movie is null)
            {
                return null;
            }

            var voteAverage = 0.0;
            var userVote = 0;

            if (await _context.MovieRatings.AnyAsync(x => x.MovieId == id))
            {
                voteAverage = await _context.MovieRatings.Where(x => x.MovieId == id)
                    .AverageAsync(x => x.Rate);

                var userId = await _authenticationStateService.GetCurrentUserId();

                if (userId is not null)
                {
                    var userVoteDb = await _context.MovieRatings
                        .FirstOrDefaultAsync(x => x.MovieId == id && x.UserId == userId);

                    if (userVoteDb != null)
                    {
                        userVote = userVoteDb.Rate;
                    }
                }
            }

            movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Order).ToList();

            var model = new DetailsMovieDTO
            {
                Movie = movie,
                Genres = movie.MoviesGenres.Select(x => x.Genre).ToList(),
                Actors = movie.MoviesActors.Select(x => 
                    new Person
                    {
                        Name = x.Person.Name,
                        Picture = x.Person.Picture,
                        Character = x.Character,
                        Id = x.PersonId
                    }).ToList(),
                UserVote = userVote,
                AverageVote = voteAverage
            };

            return model;
        }

        public async Task<IndexPageDTO> GetIndexPageDTO()
        {
            var limit = 6;

            var moviesInTheaters = await _context.Movies
                .Where(x => x.InTheaters).Take(limit)
                .OrderByDescending(x => x.ReleaseDate)
                .AsNoTracking()
                .ToListAsync();

            var todaysDate = DateTime.Today;

            var upcomingReleases = await _context.Movies
                .Where(x => x.ReleaseDate > todaysDate)
                .OrderBy(x => x.ReleaseDate).Take(limit)
                .AsNoTracking()
                .ToListAsync();

            var response = new IndexPageDTO();
            response.Intheaters = moviesInTheaters;
            response.UpcomingReleases = upcomingReleases;

            return response;
        }

        public async Task<MovieUpdateDTO> GetMovieForUpdate(int id)
        {
            var movieDetailDto = await GetDetailsMovieDTO(id);

            if (movieDetailDto is null) return null;

            var selectedGenresIds = movieDetailDto.Genres.Select(x => x.Id).ToList();
            var notSelectedGenres = await _context.Genres
                .Where(x => !selectedGenresIds.Contains(x.Id))
                .ToListAsync();

            var model = new MovieUpdateDTO
            {
                Movie = movieDetailDto.Movie,
                SelectedGenres = movieDetailDto.Genres,
                NotSelectedGenres = notSelectedGenres,
                Actors = movieDetailDto.Actors
            };

            return model;
        }

        public async Task<PaginatedResponse<List<Movie>>> GetMoviesFiltered(FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }

            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }

            if (filterMoviesDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId)
                        .Contains(filterMoviesDTO.GenreId));
            }

            var movies = await moviesQueryable.GetPaginatedResponse(filterMoviesDTO.Pagination);

            return movies;
        }

        public async Task UpdateMovie(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Detached;

            var movieDb = await _context.Movies
                .Include(x => x.MoviesActors)
                .Include(x => x.MoviesGenres)
                .FirstOrDefaultAsync(x => x.Id == movie.Id);

            movieDb = _mapper.Map(movie, movieDb);

            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                var moviePoster = Convert.FromBase64String(movie.Poster);
                movieDb.Poster = await _fileStorageService.EditFile(moviePoster,
                    "jpg", containerName, movieDb.Poster);
            }

            if (movie.MoviesActors is not null)
            {
                for (var i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i++;
                }
            }

            movieDb.MoviesActors = movie.MoviesActors;
            movieDb.MoviesGenres = movie.MoviesGenres;

            await _context.SaveChangesAsync();
        }
    }
}
