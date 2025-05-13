using NetZone_BackEnd.Data;
using NetZone_BackEnd.Models;
using Microsoft.EntityFrameworkCore;


namespace NetZone_BackEnd.Service
{
        public interface IProgressTrackingService
        {
            Task AddOrUpdateAsync(ProgressTrackingDto dto);
            Task<List<ProgressTrackingDto>> GetByLessonAsync(int lessonId);
        }

        public class ProgressTrackingService : IProgressTrackingService
        {
            private readonly NetZoneDbContext _context;

            public ProgressTrackingService(NetZoneDbContext context)
            {
                _context = context;
            }

            public async Task AddOrUpdateAsync(ProgressTrackingDto dto)
            {
                var existing = await _context.ProgressTrackings
                    .FirstOrDefaultAsync(p => p.LessonId == dto.LessonId && p.UserId == dto.UserId);

                if (existing == null)
                {
                    var newProgress = new ProgressTracking
                    {
                        LessonId = dto.LessonId,
                        UserId = dto.UserId,
                        Note = dto.Note,
                        Evaluation = dto.Evaluation,
                        Suggestion = dto.Suggestion,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.ProgressTrackings.Add(newProgress);
                }
                else
                {
                    existing.Note = dto.Note;
                    existing.Evaluation = dto.Evaluation;
                    existing.Suggestion = dto.Suggestion;
                    existing.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            public async Task<List<ProgressTrackingDto>> GetByLessonAsync(int lessonId)
            {
                return await _context.ProgressTrackings
                    .Where(p => p.LessonId == lessonId)
                    .Select(p => new ProgressTrackingDto
                    {
                        LessonId = p.LessonId,
                        UserId = p.UserId,
                        Note = p.Note,
                        Evaluation = p.Evaluation,
                        Suggestion = p.Suggestion
                    }).ToListAsync();
            }
        }
    }


