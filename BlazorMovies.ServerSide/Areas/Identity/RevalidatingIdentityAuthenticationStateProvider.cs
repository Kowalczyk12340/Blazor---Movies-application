using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlazorMovies.ServerSide.Areas.Identity
{
    public class RevalidatingIdentityAuthenticationStateProvider<TUser> : RevalidatingServerAuthenticationStateProvider where TUser : class
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IdentityOptions _options;


        public RevalidatingIdentityAuthenticationStateProvider(ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<IdentityOptions> optionsAccessor) : base(loggerFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _options = optionsAccessor.Value;
        }

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var scope = _serviceScopeFactory.CreateScope();
            try
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                return await ValidateSecurityStampAsync(userManager, authenticationState.User);
            }
            finally
            {
                if (scope is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    scope.Dispose();
                }
            }
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);

            if (user == null)
            {
                return false;
            }

            if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }

            var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
            var userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp == userStamp;
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);
    }
}
