using System;

namespace WhatsappSaaS.Application.Common.Interfaces;

public interface ITenantService
{
    Guid CompanyId { get; }
}
