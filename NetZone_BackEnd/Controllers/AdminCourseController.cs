using Microsoft.AspNetCore.Mvc;
using NetZone_BackEnd.Models;
using NetZone_BackEnd.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace NetZone_BackEnd.Controllers
{
    [Route("api/admin/courses")]
    [ApiController]
    public class AdminCourseController : ControllerBase
    {
        private readonly NetZoneDbContext _context;

        public AdminCourseController(NetZoneDbContext context)
        {
            _context = context;
        }

        // POST: api/admin/courses
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // PUT: api/admin/courses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest("Course ID mismatch");
            }

            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null)
            {
                return NotFound("Course not found");
            }

            existingCourse.Title = course.Title;
            existingCourse.Description = course.Description;
            existingCourse.Fee = course.Fee;
            existingCourse.MaxStudents = course.MaxStudents;
            existingCourse.CoachId = course.CoachId;
            existingCourse.CourseCategoryId = course.CourseCategoryId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingCourse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        // XEM CHI TIET KHOA HOC
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseCategory)
                .Include(c => c.Coach)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return NotFound("Course not found");

            return Ok(course);
        }
        //  Số lượng học viên còn trống 
        [HttpGet("remaining-slots/{courseId}")]
        public async Task<IActionResult> GetRemainingSlots(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound("Course not found");

            var registeredCount = await _context.CourseRegistrations
                .Where(r => r.CourseId == courseId)
                .CountAsync();

            int remaining = course.MaxStudents - registeredCount;

            return Ok(new
            {
                CourseId = courseId,
                MaxStudents = course.MaxStudents,
                Registered = registeredCount,
                RemainingSlots = remaining
            });
        }


        // DELETE: api/admin/courses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}