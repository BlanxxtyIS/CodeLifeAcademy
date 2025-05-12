using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Application.Validators;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeLifeAcademy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicsController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IValidator<CreateTopicDto> _createTopicValidator;

    public TopicsController(ApplicationDbContext context, IValidator<CreateTopicDto> createTopicValidator)
    {
        _context = context;
        _createTopicValidator = createTopicValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Topic>>> GetTopics()
    {
        try
        {
            var topics = await _context.Topics
                .Include(t => t.Course)
                .OrderBy(l => l.Order)
                .ToListAsync();

            return Ok(topics);
        } 
        catch (Exception ex)
        {
            return BadRequest($"Не удалось получить темы: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Topic>> GetTopic(Guid id)
    {
        var topic = await _context.Topics.FindAsync(id);

        return (topic is null) ? 
            NotFound() : Ok(topic);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Topic>> AddTopic([FromBody] CreateTopicDto dto)
    {
        var validationResult = await _createTopicValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var topic = new Topic
        {
            Title = dto.Title,
            Description = dto.Description,
            CourseId = dto.CourseId,
            Order = dto.Order
        };

        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTopic), new { id = topic.Id }, topic);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] CreateTopicDto dto)
    {
        var validationResult = await _createTopicValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var topic = await _context.Topics.FindAsync(id);

        if (topic is null || id != topic.Id)
        {
            return NotFound();
        }

        topic.Title = dto.Title;
        topic.Description = dto.Description;
        topic.Order = dto.Order;

        _context.Entry(topic).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        } 
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("Тема уже была обновлена другим администратором. Пожалуйста, перезагружите данные.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTopic(Guid id)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic is null)
        {
            return NotFound();
        }

        _context.Topics.Remove(topic);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
