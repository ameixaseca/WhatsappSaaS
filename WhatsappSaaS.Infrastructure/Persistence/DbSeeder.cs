using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WhatsappSaaS.Domain.Entities;

namespace WhatsappSaaS.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }

        // Seed Plans
        if (!await context.Plans.AnyAsync())
        {
            var freePlan = new Plan { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Free", MaxUsers = 2, MaxMessages = 500, Price = 0 };
            var basicPlan = new Plan { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Basic", MaxUsers = 5, MaxMessages = 5000, Price = 49 };
            var proPlan = new Plan { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Pro", MaxUsers = 20, MaxMessages = 50000, Price = 149 };

            context.Plans.AddRange(freePlan, basicPlan, proPlan);
            await context.SaveChangesAsync();
        }

        // Seed demo Company
        var demoCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000010");
        if (!await context.Companies.AnyAsync())
        {
            var company = new Company
            {
                Id = demoCompanyId,
                Name = "Demo Company",
                Cnpj = "00.000.000/0001-00",
                PlanId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                IsActive = true
            };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }

        // Seed admin user
        if (!await context.Users.IgnoreQueryFilters().AnyAsync())
        {
            var adminUser = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000020"),
                Name = "Admin",
                Email = "admin@demo.com",
                PasswordHash = "admin123", // NOTE: MVP only - production should use BCrypt/PBKDF2 hashing
                Role = UserRole.Admin,
                CompanyId = demoCompanyId
            };
            var attendantUser = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
                Name = "Atendente Demo",
                Email = "atendente@demo.com",
                PasswordHash = "atend123", // NOTE: MVP only - production should use BCrypt/PBKDF2 hashing
                Role = UserRole.Attendant,
                CompanyId = demoCompanyId
            };
            context.Users.AddRange(adminUser, attendantUser);
            await context.SaveChangesAsync();
        }

        // Seed demo contacts
        if (!await context.Contacts.IgnoreQueryFilters().AnyAsync())
        {
            var contacts = new[]
            {
                new Contact { Name = "João Silva", PhoneNumber = "+55 11 99999-0001", Notes = "Cliente interessado no plano Pro", CompanyId = demoCompanyId },
                new Contact { Name = "Maria Oliveira", PhoneNumber = "+55 11 99999-0002", Notes = "Lead do Instagram", CompanyId = demoCompanyId },
                new Contact { Name = "Carlos Pereira", PhoneNumber = "+55 21 98888-0001", Notes = "", CompanyId = demoCompanyId }
            };
            context.Contacts.AddRange(contacts);
            await context.SaveChangesAsync();
        }

        // Seed default tags
        if (!await context.Tags.IgnoreQueryFilters().AnyAsync())
        {
            var tags = new[]
            {
                new Tag { Name = "Novo Lead", Color = "#28a745", CompanyId = demoCompanyId },
                new Tag { Name = "Orçamento", Color = "#ffc107", CompanyId = demoCompanyId },
                new Tag { Name = "Cliente Ativo", Color = "#007bff", CompanyId = demoCompanyId },
                new Tag { Name = "Suporte", Color = "#dc3545", CompanyId = demoCompanyId },
                new Tag { Name = "Pós-Venda", Color = "#6f42c1", CompanyId = demoCompanyId }
            };
            context.Tags.AddRange(tags);
            await context.SaveChangesAsync();
        }

        // Seed quick responses
        if (!await context.QuickResponses.IgnoreQueryFilters().AnyAsync())
        {
            var qr = new[]
            {
                new QuickResponse { Shortcut = "/preco", Content = "Nossos preços variam conforme o plano. Acesse nosso site para mais informações.", CompanyId = demoCompanyId },
                new QuickResponse { Shortcut = "/horario", Content = "Nosso horário de atendimento é de segunda a sexta, das 9h às 18h.", CompanyId = demoCompanyId },
                new QuickResponse { Shortcut = "/endereco", Content = "Estamos localizados na Av. Paulista, 1000 - São Paulo, SP.", CompanyId = demoCompanyId },
                new QuickResponse { Shortcut = "/agenda", Content = "Para agendar um atendimento, por favor informe data e horário desejados.", CompanyId = demoCompanyId }
            };
            context.QuickResponses.AddRange(qr);
            await context.SaveChangesAsync();
        }
    }
}
