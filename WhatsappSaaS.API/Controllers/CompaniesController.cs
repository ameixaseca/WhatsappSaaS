using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs;
using WhatsappSaaS.Domain.Entities;
using WhatsappSaaS.Infrastructure.Persistence;

namespace WhatsappSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CompaniesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
    {
        var companies = await _context.Companies
            .IgnoreQueryFilters()
            .Include(c => c.Plan)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Cnpj = c.Cnpj,
                IsActive = c.IsActive,
                PlanId = c.PlanId,
                PlanName = c.Plan.Name,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetById(Guid id)
    {
        var company = await _context.Companies
            .IgnoreQueryFilters()
            .Include(c => c.Plan)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null) return NotFound();

        return Ok(new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Cnpj = company.Cnpj,
            IsActive = company.IsActive,
            PlanId = company.PlanId,
            PlanName = company.Plan.Name,
            CreatedAt = company.CreatedAt
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyRequest request)
    {
        var plan = await _context.Plans.FindAsync(request.PlanId);
        if (plan == null) return BadRequest("Invalid plan.");

        var company = new Company
        {
            Name = request.Name,
            Cnpj = request.Cnpj,
            PlanId = request.PlanId,
            IsActive = true
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = company.Id }, new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Cnpj = company.Cnpj,
            IsActive = company.IsActive,
            PlanId = company.PlanId,
            PlanName = plan.Name,
            CreatedAt = company.CreatedAt
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyRequest request)
    {
        var company = await _context.Companies.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
        if (company == null) return NotFound();

        company.Name = request.Name;
        company.Cnpj = request.Cnpj;
        company.PlanId = request.PlanId;
        company.IsActive = request.IsActive;
        company.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(Guid id)
    {
        var users = await _context.Users
            .IgnoreQueryFilters()
            .Where(u => u.CompanyId == id)
            .Include(u => u.Company)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.ToString(),
                CompanyId = u.CompanyId,
                CompanyName = u.Company.Name,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("{id}/users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> AddUser(Guid id, [FromBody] CreateUserRequest request)
    {
        var company = await _context.Companies.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
        if (company == null) return NotFound("Company not found.");

        if (!Enum.TryParse<UserRole>(request.Role, out var role))
            role = UserRole.Attendant;

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = request.Password, // Simplified for MVP
            Role = role,
            CompanyId = id
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            CompanyId = user.CompanyId,
            CompanyName = company.Name,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpGet("plans")]
    public async Task<ActionResult<IEnumerable<PlanDto>>> GetPlans()
    {
        var plans = await _context.Plans
            .Select(p => new PlanDto
            {
                Id = p.Id,
                Name = p.Name,
                MaxUsers = p.MaxUsers,
                MaxMessages = p.MaxMessages,
                Price = p.Price
            })
            .ToListAsync();
        return Ok(plans);
    }
}
