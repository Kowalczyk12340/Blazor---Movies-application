using System.Security.Claims;
using BlazorMovies.SharedBackend.Helpers;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorMovies.ServerSide.Helpers
{
    public class AuthenticationStateServiceServerSide : IAuthenticationStateService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationStateServiceServerSide(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> GetCurrentUserId()
        {
            var userState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            if (!userState.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var claims = userState.User.Claims;

            var claimWithUserId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (claimWithUserId == null)
            {
                throw new ApplicationException("Could not find User's ID");
            }

            return claimWithUserId.Value;
        }
    }
}