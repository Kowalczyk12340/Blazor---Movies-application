using Microsoft.EntityFrameworkCore;
using BlazorMovies.Shared.Entities;
using BlazorMovies.Shared.Repositories;

namespace BlazorMovies.SharedBackend.Repositories
{
    public class GenresRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenresRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateGenre(Genre genre)
        {
            await _context.AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public async Task<Genre> GetGenre(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<List<Genre>> GetGenres()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task UpdateGenre(Genre genre)
        {
            _context.Attach(genre).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGenre(int id)
        {
            var genre = await GetGenre(id);
            _context.Remove(genre);
            await _context.SaveChangesAsync();
        }
    }
}
