using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs;
using WhatsappSaaS.Application.Common.Interfaces;
using WhatsappSaaS.Domain.Entities;
using WhatsappSaaS.Infrastructure.Persistence;

namespace WhatsappSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public ContactsController(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetAll()
    {
        var contacts = await _context.Contacts
            .Include(c => c.Tags)
            .Select(c => new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                Notes = c.Notes,
                CompanyId = c.CompanyId,
                Tags = c.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Color = t.Color, CompanyId = t.CompanyId }).ToList(),
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(contacts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetById(Guid id)
    {
        var contact = await _context.Contacts
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null) return NotFound();

        return Ok(new ContactDto
        {
            Id = contact.Id,
            Name = contact.Name,
            PhoneNumber = contact.PhoneNumber,
            Notes = contact.Notes,
            CompanyId = contact.CompanyId,
            Tags = contact.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Color = t.Color, CompanyId = t.CompanyId }).ToList(),
            CreatedAt = contact.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactRequest request)
    {
        var contact = new Contact
        {
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Notes = request.Notes
        };

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, new ContactDto
        {
            Id = contact.Id,
            Name = contact.Name,
            PhoneNumber = contact.PhoneNumber,
            Notes = contact.Notes,
            CompanyId = contact.CompanyId,
            CreatedAt = contact.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactRequest request)
    {
        var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();

        contact.Name = request.Name;
        contact.PhoneNumber = request.PhoneNumber;
        contact.Notes = request.Notes;
        contact.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/tags/{tagId}")]
    public async Task<IActionResult> AddTag(Guid id, Guid tagId)
    {
        var contact = await _context.Contacts.Include(c => c.Tags).FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound("Contact not found.");

        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);
        if (tag == null) return NotFound("Tag not found.");

        contact.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}/tags/{tagId}")]
    public async Task<IActionResult> RemoveTag(Guid id, Guid tagId)
    {
        var contact = await _context.Contacts.Include(c => c.Tags).FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound("Contact not found.");

        var tag = contact.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag == null) return NotFound("Tag not found on contact.");

        contact.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
