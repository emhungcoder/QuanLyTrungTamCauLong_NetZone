using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetZone_BackEnd.Models;
using NetZone_BackEnd.Service;

namespace NetZone_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingMaterialController : ControllerBase
    {
        private readonly ITrainingMaterialService _service;

        public TrainingMaterialController(ITrainingMaterialService service)
        {
            _service = service;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var materials = await _service.GetByCourseAsync(courseId);
            return Ok(materials);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrainingMaterialDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByCourse), new { courseId = created.CourseId }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? Ok() : NotFound();
        }
    }

}
