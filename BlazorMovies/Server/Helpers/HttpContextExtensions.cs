using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.Server.Helpers
{
    public static class HttpContextExtensions
    {
        public static async Task InsertPaginationParametersInResponse<T>(this HttpContext httpContext,
            IQueryable<T> queryable, int recordsPerPage)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

            double count = await queryable.CountAsync();
            double totalAmountPages = Math.Ceiling(count / recordsPerPage);
            httpContext.Response.Headers.Add("totalAmountPages", totalAmountPages.ToString());
        }

        public static void InsertPaginationParametersInResponse(this HttpContext httpContext,
            int totalAmountPages)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

            httpContext.Response.Headers.Add("totalAmountPages", totalAmountPages.ToString());
        }
    }
}
