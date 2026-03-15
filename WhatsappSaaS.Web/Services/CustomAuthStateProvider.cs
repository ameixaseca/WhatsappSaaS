using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WhatsappSaaS.Web.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly AuthStateService _authStateService;

    public CustomAuthStateProvider(AuthStateService authStateService)
    {
        _authStateService = authStateService;
        _authStateService.OnChange += NotifyAuthStateChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_authStateService.IsAuthenticated)
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, _authStateService.UserName ?? ""),
            new Claim(ClaimTypes.Email, _authStateService.UserEmail ?? ""),
            new Claim(ClaimTypes.Role, _authStateService.Role ?? ""),
            new Claim("CompanyId", _authStateService.CompanyId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(user));
    }

    private void NotifyAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
