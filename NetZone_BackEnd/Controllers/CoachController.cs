using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetZone_BackEnd.Models;
using NetZone_BackEnd.Service;


namespace NetZone_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Coach")]
    public class CoachController : ControllerBase // Xem lịch dạy
    {
        private readonly ICoachService _coachService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoachController(ICoachService coachService, UserManager<ApplicationUser> userManager)
        {
            _coachService = coachService;
            _userManager = userManager;
        }

        // GET: api/coach/schedule?view=week|month
        [HttpGet("schedule")]
        public async Task<IActionResult> GetSchedule([FromQuery] string view = "week")
        {
            var userId = _userManager.GetUserId(User);
            var result = await _coachService.GetScheduleAsync(userId, view);
            return Ok(result);
        }

        // GET: api/coach/upcoming?minutes=60
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming([FromQuery] int minutes = 60)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _coachService.GetUpcomingLessonsAsync(userId, TimeSpan.FromMinutes(minutes));
            return Ok(result);
        }
    }

}
