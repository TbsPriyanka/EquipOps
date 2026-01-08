using EquipOps.BAL.Interfaces;
using EquipOps.Model.AuthLogin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipOps.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, ILogger<AuthController> _iLogger) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequestViewModel? request)
        {
            _iLogger.LogInformation("API hit: Login. Email={Email}", request?.Email);

            var result = await _authService.UserLoginAsync(request);

            _iLogger.LogInformation("API Response For Email={Email}: Success={Success}", request?.Email, result.Success);

            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordAuthRequest request)
        {
            _iLogger.LogInformation("API hit: Login. Email={Email}", request?.Email);

            var result = await _authService.ForgotPasswordAsync(request?.Email ?? string.Empty);

            _iLogger.LogInformation("API response for Email={Email}: Success={Success}", request?.Email, result.Success);

            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            _iLogger.LogInformation("API Hit: ResetPassword. Token={Token}", request.Token);

            var result = await _authService.ResetPasswordAsync(request);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            _iLogger.LogInformation("API hit: ChangePassword For Email={Email}", request.Email);

            var result = await _authService.ChangePasswordAsync(request);

            _iLogger.LogInformation("API hit: Password Changed For Email={Email}", request.Email);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            _iLogger.LogInformation("API Hit: Logout For UserId={UserId}", request?.UserId);

            var result = await _authService.LogoutAsync(request!);

            _iLogger.LogInformation("Logout Response For UserId={UserId}: Success={Success}",
                request?.UserId, result.Success);

            return StatusCode(result.StatusCode, result);
        }
    }
}
