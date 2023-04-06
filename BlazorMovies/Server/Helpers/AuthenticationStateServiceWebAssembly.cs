using BlazorMovies.SharedBackend.Helpers;
using Microsoft.AspNetCore.Identity;

namespace BlazorMovies.Server.Helpers
{
    public class AuthenticationStateServiceWebAssembly : IAuthenticationStateService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthenticationStateServiceWebAssembly(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<string> GetCurrentUserId()
        {
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext.User.Identity.Name);

            return user.Id;
        }
    }
}
