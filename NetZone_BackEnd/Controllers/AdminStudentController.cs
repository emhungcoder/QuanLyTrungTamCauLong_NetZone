using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetZone_BackEnd.Data;
using NetZone_BackEnd.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetZone_BackEnd.Controllers.Admin
{
    [Route("api/admin/students")]
    [ApiController]
    public class AdminStudentController : ControllerBase
    {
        private readonly NetZoneDbContext _context;

        public AdminStudentController(NetZoneDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/students
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _context.Users
                .Select(s => new
                {
                    s.Id,
                    s.FullName,
                    s.Email,
                    s.DateOfBirth,
                    s.IsActive
                })
                .ToListAsync();

            return Ok(students);
        }

        // GET: api/admin/students/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(string id)
        {
            var student = await _context.Users
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.FullName,
                    s.Email,
                    s.DateOfBirth,
                    s.IsActive
                })
                .FirstOrDefaultAsync();

            if (student == null) return NotFound("Student not found");

            return Ok(student);
        }

        // GET: api/admin/students/course/{courseId}
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetStudentsByCourse(int courseId)
        {
            var students = await _context.CourseRegistrations
                .Where(cr => cr.CourseId == courseId)
                .Select(cr => new
                {
                    cr.UserId,
                    cr.User.FullName,
                    cr.User.Email,
                    cr.RegistrationDate
                })
                .ToListAsync();

            return Ok(students);
        }

        // PUT: api/admin/students/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStudentStatus(string id, [FromBody] UpdateStudentStatusDto dto)
        {
            var student = await _context.Users.FindAsync(id);
            if (student == null) return NotFound("Student not found");

            student.IsActive = dto.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Student status updated successfully" });
        }

        // DELETE: api/admin/students/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await _context.Users.FindAsync(id);
            if (student == null) return NotFound("Student not found");

            _context.Users.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Student deleted successfully", StudentId = id });
        }

        // DTO
        public class UpdateStudentStatusDto
        {
            public string IsActive { get; set; }
        }
    }
}
