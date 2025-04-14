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
        var topics = await _context.Topics.ToListAsync();
        return Ok(topics);
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
            CourseId = dto.CourseId
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

        _context.Topics.Update(topic);
        await _context.SaveChangesAsync();

        return NoContent();
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
