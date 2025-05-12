using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetZone_BackEnd.Data;
using NetZone_BackEnd.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetZone_BackEnd.Controllers.Admin
{
    [Route("api/admin/classes")]
    [ApiController]
    public class AdminClassController : ControllerBase
    {
        private readonly NetZoneDbContext _context;

        public AdminClassController(NetZoneDbContext context)
        {
            _context = context;
        }

        // CREATE LESSON
        [HttpPost("create-lesson")]
        public async Task<IActionResult> CreateLesson([FromBody] LessonDto lessonDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var lesson = new Lesson
                {
                    CourseId = lessonDto.CourseId,
                    StartTime = lessonDto.StartTime,
                    EndTime = lessonDto.EndTime,
                    Location = lessonDto.Location
                };

                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetLesson", new { id = lesson.LessonId }, lesson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET LESSONS BY COURSE
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLessonsByCourse(int courseId)
        {
            try
            {
                var lessons = await _context.Lessons
                    .Where(l => l.CourseId == courseId)
                    .Select(l => new
                    {
                        l.LessonId,
                        l.CourseId,
                        l.StartTime,
                        l.EndTime,
                        l.Location
                    })
                    .ToListAsync();

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // UPDATE LESSON
        [HttpPut("update-lesson/{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] LessonDto lessonDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var lesson = await _context.Lessons.FindAsync(id);
                if (lesson == null) return NotFound("Lesson not found");

                lesson.StartTime = lessonDto.StartTime;
                lesson.EndTime = lessonDto.EndTime;
                lesson.Location = lessonDto.Location;

                await _context.SaveChangesAsync();
                return Ok(lesson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE LESSON
        [HttpDelete("delete-lesson/{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            try
            {
                var lesson = await _context.Lessons.FindAsync(id);
                if (lesson == null) return NotFound("Lesson not found");

                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Lesson deleted successfully", LessonId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DTO
        public class LessonDto
        {
            public int CourseId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Location { get; set; }
        }
    }
}
