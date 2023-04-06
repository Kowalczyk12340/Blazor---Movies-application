using BlazorMovies.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.SharedBackend.Helpers
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResponse<List<T>>> GetPaginatedResponse<T>(
            this IQueryable<T> queryable,
            PaginationDTO paginationDto)
        {
            double count = await queryable.CountAsync();
            var totalAmountOfPages = (int)Math.Ceiling(count / paginationDto.RecordsPerPage);
            var records = await queryable.Paginate(paginationDto).ToListAsync();
            var response = new PaginatedResponse<List<T>>
            {
                TotalAmountPages = totalAmountOfPages,
                Response = records
            };

            return response;
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO paginationDto)
        {
            return queryable
                .Skip((paginationDto.Page - 1) * paginationDto.RecordsPerPage)
                .Take(paginationDto.RecordsPerPage);
        }
    }
}
