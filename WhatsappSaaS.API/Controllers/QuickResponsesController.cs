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
public class QuickResponsesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuickResponsesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuickResponseDto>>> GetAll()
    {
        var qrs = await _context.QuickResponses
            .Select(q => new QuickResponseDto { Id = q.Id, Shortcut = q.Shortcut, Content = q.Content, CompanyId = q.CompanyId })
            .ToListAsync();
        return Ok(qrs);
    }

    [HttpPost]
    public async Task<ActionResult<QuickResponseDto>> Create([FromBody] CreateQuickResponseRequest request)
    {
        var qr = new QuickResponse { Shortcut = request.Shortcut, Content = request.Content };
        _context.QuickResponses.Add(qr);
        await _context.SaveChangesAsync();

        return Ok(new QuickResponseDto { Id = qr.Id, Shortcut = qr.Shortcut, Content = qr.Content, CompanyId = qr.CompanyId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuickResponseRequest request)
    {
        var qr = await _context.QuickResponses.FirstOrDefaultAsync(q => q.Id == id);
        if (qr == null) return NotFound();

        qr.Shortcut = request.Shortcut;
        qr.Content = request.Content;
        qr.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var qr = await _context.QuickResponses.FirstOrDefaultAsync(q => q.Id == id);
        if (qr == null) return NotFound();

        _context.QuickResponses.Remove(qr);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
