using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs.Auth;

namespace WhatsappSaaS.Web.Services;

public class AuthStateService
{
    public string? Token { get; private set; }
    public string? UserName { get; private set; }
    public string? UserEmail { get; private set; }
    public string? Role { get; private set; }
    public Guid CompanyId { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    public event Action? OnChange;

    public void SetAuth(AuthResponse response)
    {
        Token = response.Token;
        UserName = response.Name;
        UserEmail = response.Email;
        Role = response.Role;
        CompanyId = response.CompanyId;
        NotifyStateChanged();
    }

    public void Logout()
    {
        Token = null;
        UserName = null;
        UserEmail = null;
        Role = null;
        CompanyId = Guid.Empty;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
