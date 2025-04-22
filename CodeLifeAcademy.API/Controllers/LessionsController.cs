using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeLifeAcademy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LessionsController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IValidator<CreateLessionDto> _createLessionValidator;

    public LessionsController(ApplicationDbContext context, IValidator<CreateLessionDto> createLessionValidator)
    {
        _context = context;
        _createLessionValidator = createLessionValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Lession>>> GetLessions()
    {
        var lessons = await _context.Lessions
            .OrderBy(l => l.Order)
            .ToListAsync();

        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Lession>> GetLession(Guid id)
    {
        var lession = await _context.Lessions.FindAsync(id);
        if (lession is null) return NotFound();
        return Ok(new
        {
            lession.Id,
            lession.Title,
            lession.Order,
            Content = lession.Content // HTML остаётся как есть, но обёрнут в JSON
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Lession>> AddLession([FromBody] CreateLessionDto dto)
    {
        var validateResult = _createLessionValidator.Validate(dto);
        if (!validateResult.IsValid)
        {
            return BadRequest(validateResult.Errors);
        }

        var lession = new Lession()
        {
            Title = dto.Title,
            Content = dto.Content,
            TopicId = dto.TopicId,
            Order = dto.Order
        };

        _context.Lessions.Add(lession);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLession), new { id = lession.Id }, lession);

    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ManageCourses")]
    public async Task<ActionResult> UpdateLession(Guid id, [FromBody] CreateLessionDto dto)
    {
        var validationResult = _createLessionValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var lession = await _context.Lessions.FindAsync(id);
        if (lession is null || lession.Id != id)
        {
            return NotFound();
        }

        lession.Title = dto.Title;
        lession.Content = dto.Content;
        lession.Order = dto.Order;

        _context.Entry(lession).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("Урок уже был обновлен другим администратором. Пожжалуйста, перезагружите данные.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManageCourses")]
    public async Task<ActionResult> DeleteLession(Guid id)
    {
        var lession = await _context.Lessions.FindAsync(id);
        if (lession is null)
        {
            return NotFound();
        }

        _context.Lessions.Remove(lession);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
