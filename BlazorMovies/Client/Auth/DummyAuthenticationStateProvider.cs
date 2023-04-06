using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorMovies.Client.Auth
{
    public class DummyAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonymous = new ClaimsIdentity(new List<Claim>
            {
                new Claim("key1", "value1"),
                new Claim(ClaimTypes.Name, "Felipe"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "test");

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
        }
    }
}
