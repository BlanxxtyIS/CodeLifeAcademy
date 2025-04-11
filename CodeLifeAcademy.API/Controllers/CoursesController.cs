using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeLifeAcademy.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<CreateCourseDto> _createCourseValidator;

        public CoursesController(ApplicationDbContext context, IValidator<CreateCourseDto> createCourseValidator)
        {
            _context = context;
            _createCourseValidator = createCourseValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return courses;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);

            return (course is null) ?
                NotFound() : Ok(course);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Course>> CreateCourse([FromBody] CreateCourseDto dto)
        {
            var validationResult = await _createCourseValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync(); 

            return CreatedAtAction(nameof(GetCourse), 
                new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CreateCourseDto dto)
        {
            var validationResult = await _createCourseValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var course = await _context.Courses.FindAsync(id);

            if (course is null || id != course.Id)
            {
                return NotFound();
            }

            course.Title = dto.Title;
            course.Description = dto.Description;

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course is null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
