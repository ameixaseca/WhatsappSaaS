using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs.Auth;

namespace WhatsappSaaS.Application.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
