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
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ApplicationDbContext context, 
            IValidator<CreateCourseDto> createCourseValidator,
            ILogger<CoursesController> logger)
        {
            _context = context;
            _createCourseValidator = createCourseValidator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            try
            {
                _logger.LogInformation("Получение всех курсов начато");

                var courses = await _context.Courses
                    .Include(c => c.Topics)
                    .ToListAsync();

                _logger.LogInformation($"Получено {courses.Count} курсов");
                return Ok(courses);
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Ошибка при получении списка курсов");
                return StatusCode(500, "Произошла ошибка на сервере при загрузке курсов.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(Guid id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);

                return (course is null) ?
                    NotFound() : Ok(course);
            } 
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка на сервере при загрузке курса.");
            }
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

            _logger.LogInformation("Создание нового курса: {Title}", dto.Title);
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
            course.Image = dto.Image;
            course.Progress = dto.Progress;
            course.TimeInMinutes = dto.TimeInMinutes;

            _context.Entry(course).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Курс уже был обновлен другим администратором. Пожжалуйста, перезагружите данные.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course is null)
                {
                    _logger.LogWarning($"Попытка удаления несуществующего курса: {id}");
                    return NotFound();
                }

                _logger.LogInformation($"Курс {id} удален!");
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                return NoContent();
            } 
            catch (Exception ex)
            {
                _logger.LogError($"Ошкибка при удалени икурса: {ex.Message}");
                return BadRequest($"Ошибка при удалении курса: {ex.Message}");
            }
        }
    }
}
