using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NetZone_BackEnd.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NetZone_BackEnd.Data;
namespace ASM.API.Controllers
{
   
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly NetZoneDbContext _context ;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            NetZoneDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tạo user mới từ model đăng ký
            var user = new ApplicationUser
            {
                IsActive = "on",
                Id = Guid.NewGuid().ToString(),  // Gán Id mới
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                //Address = model.Address,
                PhoneNumber = model.PhoneNumber  // Gán số điện thoại
            };
       
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                // Trả về lỗi nếu đăng ký thất bại
                return BadRequest(result.Errors);
            }
            if (!string.IsNullOrEmpty(model.Address))
            {
                var address = new UserAddress
                {
                    Address = model.Address,
                    IsDefault = true,
                    UserId = user.Id
                };
                _context.UserAddresses.Add(address);
                await _context.SaveChangesAsync();
            }
            await _userManager.AddToRoleAsync(user, "Student");

            return Ok(new { message = "Đăng ký thành công!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Sai email hoặc mật khẩu!" });

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Sai email hoặc mật khẩu!" });
            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault() ?? "";
            var token = GenerateJwtToken(user, roleName);
            return Ok(new { token });
        }
        

        private string GenerateJwtToken(ApplicationUser user, string roleName)
        {

            var claims = new[]
            {
    new Claim(JwtRegisteredClaimNames.Sub, user.Id),                   
    new Claim(ClaimTypes.NameIdentifier, user.Id),               
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    new Claim("FullName", user.FullName),
    new Claim(ClaimTypes.Role, roleName)
};


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // Lấy thông tin cá nhân của user hiện tại
        [Authorize]
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            // Lấy user id từ User.Identity.Name (đã được map từ claim "sub")
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
              ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                // Debug: in ra danh sách claim
                var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                Console.WriteLine(JsonConvert.SerializeObject(allClaims));
                Console.WriteLine("id bi null");
                return Unauthorized(new { message = "Không có thông tin user id trong token.", claims = allClaims });
            }

            var user = await _userManager.FindByIdAsync(userId);
            //if (user == null)
            //    return NotFound();

            var userProfile = new
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                //Address = user.Address,
                Phone = user.PhoneNumber,
                RoleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
               
            };

            return Ok(userProfile);
        }


        // Cập nhật thông tin cá nhân
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Kiểm tra trùng email
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != user.Id)
                return BadRequest(new { message = "Email đã tồn tại." });

            user.FullName = model.FullName;
            user.Email = model.Email;
            //user.Address = model.Address;
            user.PhoneNumber = model.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Thông tin cá nhân đã được cập nhật thành công." });
        }

        // Đổi mật khẩu
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            Console.WriteLine("changePass called");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.NewPassword != model.ConfirmPassword)
                return BadRequest(new { message = "Mật khẩu mới và xác nhận mật khẩu không khớp." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Mật khẩu đã được thay đổi thành công." });
        }
    }
}
