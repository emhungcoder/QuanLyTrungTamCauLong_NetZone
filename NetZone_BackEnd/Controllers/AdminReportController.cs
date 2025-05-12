using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetZone_BackEnd.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetZone_BackEnd.Controllers.Admin
{
    [Route("api/admin/reports")]
    [ApiController]
    public class AdminReportController : ControllerBase
    {
        private readonly NetZoneDbContext _context;

        public AdminReportController(NetZoneDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/reports/students-by-course/{courseId}
        [HttpGet("students-by-course/{courseId}")]
        public async Task<IActionResult> GetStudentCountByCourse(int courseId)
        {
            var studentCount = await _context.CourseRegistrations
                .Where(cr => cr.CourseId == courseId)
                .CountAsync();

            return Ok(new { CourseId = courseId, StudentCount = studentCount });
        }

        // GET: api/admin/reports/revenue
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport()
        {
            var revenue = await _context.CourseRegistrations
                .Include(cr => cr.Course)
                .SumAsync(cr => cr.Course.Fee);

            return Ok(new { TotalRevenue = revenue });
        }

        // GET: api/admin/reports/attendance/{courseId}
        [HttpGet("attendance/{courseId}")]
        public async Task<IActionResult> GetAttendanceReport(int courseId)
        {
            var attendance = await _context.Attendances
                .Where(a => a.Lesson.CourseId == courseId)
                .GroupBy(a => a.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(attendance);
        }

        // GET: api/admin/reports/course-completion
        [HttpGet("course-completion")]
        public async Task<IActionResult> GetCourseCompletionRate()
        {
            var totalRegistrations = await _context.CourseRegistrations.CountAsync();
            var completedCourses = await _context.Certificates.CountAsync();

            var completionRate = totalRegistrations == 0 ? 0 : (double)completedCourses / totalRegistrations * 100;

            return Ok(new { CompletionRate = completionRate });
        }

        // GET: api/admin/reports/coupons-usage
        [HttpGet("coupons-usage")]
        public async Task<IActionResult> GetCouponsUsageReport()
        {
            var couponUsage = await _context.OrderCoupons
                .GroupBy(oc => oc.CouponId)
                .Select(g => new
                {
                    CouponId = g.Key,
                    UsageCount = g.Count()
                })
                .ToListAsync();

            return Ok(couponUsage);
        }
    }
}
