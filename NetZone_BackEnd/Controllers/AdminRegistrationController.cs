using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetZone_BackEnd.Data;
using NetZone_BackEnd.Models;
using System.Threading.Tasks;
using System.Linq;

namespace NetZone_BackEnd.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/registrations")]
    public class AdminRegistrationController : ControllerBase
    {
        private readonly NetZoneDbContext _context;

        public AdminRegistrationController(NetZoneDbContext context)
        {
            _context = context;
        }

        // POST: Đăng ký hộ học viên
        [HttpPost]
        public async Task<IActionResult> RegisterStudent([FromBody] CourseRegistration registration)
        {
            // Kiểm tra tồn tại
            var exists = await _context.CourseRegistrations
                .AnyAsync(r => r.UserId == registration.UserId && r.CourseId == registration.CourseId);
            if (exists)
                return BadRequest("Student already registered for this course.");

            _context.CourseRegistrations.Add(registration);
            await _context.SaveChangesAsync();
            return Ok(registration);
        }

        // DELETE: Huỷ đăng ký của học viên
        [HttpDelete("{registrationId}")]
        public async Task<IActionResult> CancelRegistration(int registrationId)
        {
            var reg = await _context.CourseRegistrations.FindAsync(registrationId);
            if (reg == null)
                return NotFound("Registration not found");

            _context.CourseRegistrations.Remove(reg);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Registration cancelled", registrationId });
        }

        // GET: Xem tất cả đăng ký
        [HttpGet]
        public async Task<IActionResult> GetAllRegistrations()
        {
            var regs = await _context.CourseRegistrations
                .Include(r => r.User)
                .Include(r => r.Course)
                .Select(r => new
                {
                    r.RegistrationId,
                    r.UserId,
                    StudentName = r.User.FullName,
                    r.CourseId,
                    CourseTitle = r.Course.Title,
                    r.RegistrationDate
                })
                .ToListAsync();

            return Ok(regs);
        }
    }
}
