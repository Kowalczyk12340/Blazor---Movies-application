using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTOs;
using BlazorMovies.Shared.Entities;
using BlazorMovies.Shared.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorMovies.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesRepository _moviesRepository;

        public MoviesController(IMoviesRepository moviesRepository)
        {
            _moviesRepository = moviesRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IndexPageDTO>> Get()
        {
            return await _moviesRepository.GetIndexPageDTO();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DetailsMovieDTO>> Get(int id)
        {
            var model = await _moviesRepository.GetDetailsMovieDTO(id);

            if (model is null)
            {
                return NotFound();
            }

            return model;
        }

        [HttpPost("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Movie>>> Filter(FilterMoviesDTO filterMoviesDto)
        {
            var paginatedResponse = await _moviesRepository.GetMoviesFiltered(filterMoviesDto);
            HttpContext.InsertPaginationParametersInResponse(paginatedResponse.TotalAmountPages);
            return paginatedResponse.Response;
        }

        [HttpGet("update/{id}")]
        public async Task<ActionResult<MovieUpdateDTO>> PutGet(int id)
        {
            var model = await _moviesRepository.GetMovieForUpdate(id);
            if (model == null) { return NotFound(); }
            return model;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Movie movie)
        {
            return await _moviesRepository.CreateMovie(movie);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Movie movie)
        {
            var movieDB = await _moviesRepository.GetDetailsMovieDTO(movie.Id);
            if (movieDB == null) { return NotFound(); }

            await _moviesRepository.UpdateMovie(movie);

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = await _moviesRepository.GetDetailsMovieDTO(id);
            if (movie == null)
            {
                return NotFound();
            }

            await _moviesRepository.DeleteMovie(id);
            return NoContent();
        }
    }
}