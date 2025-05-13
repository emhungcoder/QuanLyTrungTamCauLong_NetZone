using NetZone_BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace NetZone_BackEnd.Service
{
    public interface ICoachService
    {
        Task<List<LessonScheduleDto>> GetScheduleAsync(string userId, string viewType);
        Task<List<UpcomingLessonDto>> GetUpcomingLessonsAsync(string userId, TimeSpan timeWindow);
    }

    public class CoachService : ICoachService
    {
        private readonly NetZoneDbContext _context;  // Sử dụng NetZoneDbContext thay vì ApplicationDbContext

        public CoachService(NetZoneDbContext context)  // Inject NetZoneDbContext
        {
            _context = context;
        }

        public async Task<List<LessonScheduleDto>> GetScheduleAsync(string userId, string viewType)
        {
            var coach = await _context.Coaches.FirstOrDefaultAsync(c => c.UserId == userId);
            if (coach == null) return new List<LessonScheduleDto>();

            DateTime now = DateTime.UtcNow;
            DateTime end = viewType.ToLower() switch
            {
                "week" => now.AddDays(7),
                "month" => now.AddMonths(1),
                _ => now.AddDays(7)
            };

            // Truy vấn lessons dựa trên CoachId
            var lessons = await _context.CourseCoaches
                .Where(cc => cc.CoachId == coach.CoachId) // Sử dụng CoachId thay vì Id
                .SelectMany(cc => cc.Course.Lessons)
                .Where(l => l.StartTime >= now && l.StartTime <= end)
                .Select(l => new LessonScheduleDto
                {
                    LessonId = l.LessonId,  // Sử dụng LessonId thay vì Id
                    CourseName = l.Course.Title,  // Thay Course.Name thành Course.Title
                    Title = l.Location,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime
                })
                .OrderBy(l => l.StartTime)
                .ToListAsync();

            return lessons;
        }

        public async Task<List<UpcomingLessonDto>> GetUpcomingLessonsAsync(string userId, TimeSpan timeWindow)
        {
            var coach = await _context.Coaches.FirstOrDefaultAsync(c => c.UserId == userId);
            if (coach == null) return new List<UpcomingLessonDto>();

            DateTime now = DateTime.UtcNow;
            DateTime window = now.Add(timeWindow);

            // Truy vấn upcoming lessons dựa trên CoachId
            var upcoming = await _context.CourseCoaches
                .Where(cc => cc.CoachId == coach.CoachId) // Sử dụng CoachId thay vì Id
                .SelectMany(cc => cc.Course.Lessons)
                .Where(l => l.StartTime > now && l.StartTime <= window)
                .Select(l => new UpcomingLessonDto
                {
                    LessonId = l.LessonId,
                    CourseName = l.Course.Title,
                    StartTime = l.StartTime,
                    TimeUntilStart = l.StartTime - now
                })
                .OrderBy(l => l.StartTime)
                .ToListAsync();

            return upcoming;
        }
    }

    // DTO cho Lesson Schedule
    public class LessonScheduleDto
    {
        public int LessonId { get; set; }
        public string CourseName { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    // DTO cho Upcoming Lesson
    public class UpcomingLessonDto
    {
        public int LessonId { get; set; }
        public string CourseName { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan TimeUntilStart { get; set; }
    }
}
