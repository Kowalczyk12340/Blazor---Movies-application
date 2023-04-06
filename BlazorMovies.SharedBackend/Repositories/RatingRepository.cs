using BlazorMovies.Shared.Entities;
using BlazorMovies.Shared.Repositories;
using BlazorMovies.SharedBackend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.SharedBackend.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IAuthenticationStateService _authenticationStateService;
        private readonly ApplicationDbContext _context;

        public RatingRepository(IAuthenticationStateService authenticationStateService, ApplicationDbContext context)
        {
            _authenticationStateService = authenticationStateService;
            _context = context;
        }

        public async Task Vote(MovieRating movieRating)
        {
            var userId = await _authenticationStateService.GetCurrentUserId();

            if (userId is null)
            {
                return;
            }

            var currentRating = await _context.MovieRatings
                .FirstOrDefaultAsync(x => x.MovieId == movieRating.MovieId &&
                                          x.UserId == userId);

            if (currentRating is null)
            {
                movieRating.UserId = userId;
                movieRating.RatingDate = DateTime.UtcNow;
                await _context.AddAsync(movieRating);
                await _context.SaveChangesAsync();
            }
            else
            {
                currentRating.Rate = movieRating.Rate;
                await _context.SaveChangesAsync();
            }
        }
    }
}