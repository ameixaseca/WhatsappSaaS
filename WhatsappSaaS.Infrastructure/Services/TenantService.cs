using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using WhatsappSaaS.Application.Common.Interfaces;

namespace WhatsappSaaS.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid CompanyId
    {
        get
        {
            var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("CompanyId")?.Value;
            if (Guid.TryParse(companyIdClaim, out var companyId))
            {
                return companyId;
            }
            return Guid.Empty; // For system operations or initial setup
        }
    }
}
