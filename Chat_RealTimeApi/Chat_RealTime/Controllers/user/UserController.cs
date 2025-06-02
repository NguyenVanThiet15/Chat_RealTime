using Chat_RealTime.Controllers.user.Dtos;
using Chat_RealTime.Resources;
using Chat_RealTime.Services.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Chat_RealTime.Controllers.user
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtServicer _jwtServicer;
        private readonly IStringLocalizer<ShareResource> _localizer;

        public UserController(IUserService userService, IJwtServicer jwtService,IStringLocalizer<ShareResource> localizer)
        {
            _userService = userService;
            _jwtServicer = jwtService;
            _localizer = localizer;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(ResgisterDto input)
        {
            try
            {
                var existingUser = await _userService.GetByEmailAsync(input.Email);
                if (existingUser != null)
                {
                    return BadRequest("Email đã đc sử dụng!!");
                }
                var user = await _userService.CreateUserAsync(input);
                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
          
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto input)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(input.Email);
                //var hashPW = BCrypt.Net.BCrypt.HashPassword(input.Password);
                if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.PasswordHash))
                {
                    return BadRequest(new { message = _localizer["SaiEmailHoacMatKhau"].Value });
                }
                var Token = _jwtServicer.GenerateToken(user);
                return Ok(new LoginOutput
                {
                    Token = Token,
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("getListUser")]
        public async Task<IActionResult> GetListUser( string userId)
       {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Id trống");
            }
            var user = await _userService.GetListUser(userId);
            return Ok(user);
        }
        public class GoogleLoginRequest
        {
            public string TokenGoogle { get; set; } = "";
        }
        [HttpPost("loginByGoogle")]
        public  async Task<IActionResult> LoginByGoogle(GoogleLoginRequest input)
        {
            try
            {
                var user = await _userService.LoginGoogle(input.TokenGoogle);
                var TokenGoogleOuput = _jwtServicer.GenerateToken(user);
                return Ok(new LoginOutput
                {
                    TokenGoogle = TokenGoogleOuput,
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
