using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetZone_BackEnd.Models;
using NetZone_BackEnd.Service;
using static NetZone_BackEnd.Service.ProgressTrackingService;

namespace NetZone_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressTrackingController : ControllerBase
    {
        private readonly IProgressTrackingService _service;

        public ProgressTrackingController(IProgressTrackingService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromBody] ProgressTrackingDto dto)
        {
            await _service.AddOrUpdateAsync(dto);
            return Ok(new { message = "Saved successfully" });
        }

        [HttpGet("{lessonId}")]
        public async Task<IActionResult> GetByLesson(int lessonId)
        {
            var result = await _service.GetByLessonAsync(lessonId);
            return Ok(result);
        }
    }

}
