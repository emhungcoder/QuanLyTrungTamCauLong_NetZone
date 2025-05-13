using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetZone_BackEnd.Service;

namespace NetZone_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("mark")]
        public async Task<IActionResult> MarkAttendance([FromBody] AttendanceDto dto)
        {
            // Kiểm tra ModelState để đảm bảo dữ liệu hợp lệ
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto == null)
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });

            var result = await _attendanceService.MarkAttendanceAsync(dto);

            if (result)
            {
                return Ok(new { message = "Điểm danh thành công." });
            }

            // Trả về lỗi với mã 400 nếu không thể điểm danh
            return BadRequest(new { message = "Không thể điểm danh học viên. Vui lòng kiểm tra lại thông tin." });
        }
    }
}
