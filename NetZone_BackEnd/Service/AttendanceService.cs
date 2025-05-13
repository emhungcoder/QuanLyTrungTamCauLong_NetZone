using NetZone_BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using NetZone_BackEnd.Models;
using System;

namespace NetZone_BackEnd.Service
{
    public interface IAttendanceService
    {
        Task<bool> MarkAttendanceAsync(AttendanceDto dto);
    }

    public class AttendanceDto
    {
        public int LessonId { get; set; }
        public string UserId { get; set; } // học viên
        public DateTime AttendanceTime { get; set; } // Thời gian điểm danh
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly NetZoneDbContext _context;

        public AttendanceService(NetZoneDbContext context)
        {
            _context = context;
        }

        public async Task<bool> MarkAttendanceAsync(AttendanceDto dto)
        {
            // Kiểm tra Lesson và User tồn tại
            var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.LessonId == dto.LessonId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);

            if (lesson == null || !userExists) return false;

            // Xác định trạng thái điểm danh
            string status;
            DateTime lessonStartTime = lesson.StartTime;

            if (dto.AttendanceTime <= lessonStartTime)
            {
                // Đúng giờ
                status = "Present";
            }
            else if (dto.AttendanceTime <= lessonStartTime.AddMinutes(15)) // 15 phút muộn
            {
                // Muộn
                status = "Late";
            }
            else
            {
                // Vắng
                status = "Absent";
            }

            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.LessonId == dto.LessonId && a.UserId == dto.UserId);

            if (existingAttendance != null)
            {
                // Cập nhật trạng thái nếu đã điểm danh
                existingAttendance.Status = status;
                existingAttendance.Timestamp = DateTime.UtcNow;
            }
            else
            {
                // Tạo mới bản ghi điểm danh
                var attendance = new Attendance
                {
                    LessonId = dto.LessonId,
                    UserId = dto.UserId,
                    Status = status,
                    Timestamp = DateTime.UtcNow
                };
                _context.Attendances.Add(attendance);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
