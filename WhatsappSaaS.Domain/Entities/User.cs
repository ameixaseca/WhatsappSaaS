using System;

namespace WhatsappSaaS.Domain.Entities;

public enum UserRole
{
    Admin,
    Attendant
}

public class User : MultiTenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Company Company { get; set; } = null!;
}
