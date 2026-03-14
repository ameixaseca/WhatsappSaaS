using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs.Auth;
using WhatsappSaaS.Application.Services.Auth;

namespace WhatsappSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized();
        }
        return Ok(response);
    }
}
