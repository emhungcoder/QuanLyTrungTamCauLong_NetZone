using NetZone_BackEnd.Data;
using NetZone_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace NetZone_BackEnd.Service
{
    public interface ITrainingMaterialService
    {
        Task<List<TrainingMaterialDto>> GetByCourseAsync(int courseId);
        Task<TrainingMaterialDto> CreateAsync(TrainingMaterialDto dto);
        Task<bool> DeleteAsync(int id);
    }
    public class TrainingMaterialService : ITrainingMaterialService
    {
        private readonly NetZoneDbContext _context;

        public TrainingMaterialService(NetZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<TrainingMaterialDto>> GetByCourseAsync(int courseId)
        {
            return await _context.TrainingMaterials
                .Where(t => t.CourseId == courseId)
                .Select(t => new TrainingMaterialDto
                {
                    Id = t.Id,
                    CourseId = t.CourseId,
                    Title = t.Title,
                    Description = t.Description,
                    Url = t.Url,
                    Type = t.Type
                }).ToListAsync();
        }

        public async Task<TrainingMaterialDto> CreateAsync(TrainingMaterialDto dto)
        {
            var entity = new TrainingMaterial
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                Url = dto.Url,
                Type = dto.Type
            };

            _context.TrainingMaterials.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var material = await _context.TrainingMaterials.FindAsync(id);
            if (material == null) return false;

            _context.TrainingMaterials.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
