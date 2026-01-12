using EquipOps.API.Services.Interface;
using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.AuthLogin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace EquipOps.API.Services.Implementation
{
    public class AuthService(IAuthRepository _authRepository, ILogger<AuthService> _logger, IHttpContextAccessor _httpContextAccessor, IEmailService _emailService) : IAuthService
    {
        public async Task<ApiResponse<AuthLoginResponseViewModel>> UserLoginAsync(AuthLoginRequestViewModel? model)
        {
            _logger.LogInformation("AuthService: UserLoginAsync START. Email={Email}", model?.Email);

            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return new ApiResponse<AuthLoginResponseViewModel>("Email Or Password Cannot Be Empty.",(int)ApiStatusCode.BAD_REQUEST);
            }

            var data = await _authRepository.UserLoginAsync(model);

            if (data.UserId is null)
            {
                return new ApiResponse<AuthLoginResponseViewModel>("Invalid Email Or Password.",(int)ApiStatusCode.UNAUTHORIZED);
            }

            bool validPassword = PasswordHelper.VerifyPassword(model.Password, data.HashPassword);

            if (!validPassword)
            {
                return new ApiResponse<AuthLoginResponseViewModel>("Incorrect Password.",(int)ApiStatusCode.UNAUTHORIZED);
            }

            return new ApiResponse<AuthLoginResponseViewModel>(data,"Login Successfull.",(int)ApiStatusCode.OK);
        }

        public async Task<ApiResponse<ForgotPasswordAuthResponse>> ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("ForgotPassword Initiated For Email={Email}", email);

            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning("ForgotPassword Failed. User Not Found For Email={Email}", email);

                return new ApiResponse<ForgotPasswordAuthResponse>("User Not Found.",(int)ApiStatusCode.NOT_FOUND);
            }

            string resetToken = GenerateEmailToken(email);

            bool tokenSave = await _authRepository.SaveResetTokenAsync(user, resetToken);
            if (!tokenSave)
            {
                _logger.LogError("Failed To Save Reset Token In DB For Email={Email}", email);

                return new ApiResponse<ForgotPasswordAuthResponse>("Something Went Wrong While Generating Reset Link.",(int)ApiStatusCode.INTERNAL_SERVER_ERROR);
            }

            string baseUrl = GetFrontendBaseUrl();
            string resetUrl = $"{baseUrl}/reset-password/{resetToken}";

            _logger.LogInformation("Sending Password Reset Email To {Email}", email);

            await _emailService.SendForgotPasswordEmailAsync(email, user.full_name, resetUrl);

            return new ApiResponse<ForgotPasswordAuthResponse>()
            {
                Message = "Password Reset Link Sent To Your Email.",
                Success = true,
                StatusCode = (int)ApiStatusCode.OK
            };
        }

        private string GenerateEmailToken(string email)
        {
            _logger.LogInformation("Generating Token For {Email}", email);

            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();
        }

        private string GetFrontendBaseUrl()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                _logger.LogWarning("HttpContext Is Null. Using Default Fallback http://localhost");
                return "http://localhost";
            }

            var request = context.Request;

            _logger.LogInformation("Detecting Frontend Base URL...");

            string? origin = request.Headers["Origin"].ToString();
            if (!string.IsNullOrWhiteSpace(origin))
            {
                _logger.LogInformation("Frontend Origin Detected: {Origin}", origin);
                return origin.TrimEnd('/');
            }

            string? referer = request.Headers["Referer"].ToString();
            if (!string.IsNullOrWhiteSpace(referer))
            {
                var uri = new Uri(referer);
                string refererUrl = $"{uri.Scheme}://{uri.Host}" + (uri.IsDefaultPort ? "" : $":{uri.Port}");

                _logger.LogInformation("Frontend Referer Detected: {Referer}", refererUrl);
                return refererUrl.TrimEnd('/');
            }

            string apiUrl = $"{request.Scheme}://{request.Host}";

            _logger.LogWarning("Origin & Referer Headers Missing. Using API host fallback: {ApiUrl}",apiUrl);

            return apiUrl.TrimEnd('/');
        }
        public async Task<ApiResponse<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            _logger.LogInformation("ResetPassword Request Received. Token={Token}", request.Token);

            var user = await _authRepository.GetUserByResetTokenAsync(request.Token);

            if (user == null)
            {
                _logger.LogWarning("ResetPassword Failed. Invalid or Expired Token. Token={Token}", request.Token);

                return new ApiResponse<ResetPasswordResponse>("Invalid or expired token.",(int)ApiStatusCode.NOT_FOUND);
            }

            if (user.expired_at < DateTime.UtcNow)
            {
                _logger.LogWarning("ResetPassword Failed. Token Expired For User={User}, ExpiredAt={ExpiredAt}",
                    user.email, user.expired_at);

                return new ApiResponse<ResetPasswordResponse>("Reset Token Has Expired.",(int)ApiStatusCode.UNAUTHORIZED  );
            }

            string hashedPassword = PasswordHelper.HashPassword(request.NewPassword);

            bool isUpdated = await _authRepository.UpdatePasswordAsync(request.Token, hashedPassword, user);

            if (!isUpdated)
            {
                _logger.LogError("Password Reset Failed During DB Update For User={User}", user.email);

                return new ApiResponse<ResetPasswordResponse>("Something Went Wrong During Password Reset.",(int)ApiStatusCode.INTERNAL_SERVER_ERROR);
            }

            _logger.LogInformation("Password Reset Successfully For User={User}", user.email);

            return new ApiResponse<ResetPasswordResponse>("Password Reset Successfully.",(int)ApiStatusCode.OK);
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            _logger.LogInformation("ChangePassword Request For Email={Email}", request.Email);

            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user is null)
            {
                _logger.LogWarning("ChangePassword Failed: User Not Found. Email={Email}", request.Email);

                return new ApiResponse<bool>("User Not Found.",(int)ApiStatusCode.NOT_FOUND);
            }

            if (!PasswordHelper.VerifyPassword(request.OldPassword, user.hash_password))
            {
                _logger.LogWarning("ChangePassword Failed: Old Password Incorrect For Email={Email}", request.Email);

                return new ApiResponse<bool>("Old Password Is Incorrect.",(int)ApiStatusCode.UNAUTHORIZED);
            }

            string newHashedPassword = PasswordHelper.HashPassword(request.NewPassword);

            bool isUpdated = await _authRepository.ChangePasswordAsync(user.user_id, newHashedPassword);

            if (!isUpdated)
            {
                _logger.LogError("ChangePassword Failed During DB Update For Email={Email}", request.Email);

                return new ApiResponse<bool>("Failed To Update Password.",(int)ApiStatusCode.INTERNAL_SERVER_ERROR);
            }

            _logger.LogInformation("Password Changed Successfully For Email={Email}", request.Email);

            return new ApiResponse<bool>(true,"Password Changed Successfully.",(int)ApiStatusCode.OK);
        }

        public async Task<ApiResponse<string>> LogoutAsync(LogoutRequest request)
        {
            _logger.LogInformation("Logout Request Received For UserId={UserId}", request.UserId);


            bool result = await _authRepository.LogoutAsync(request.UserId);

            if (result)
            {
                _logger.LogInformation("Logout Successful For UserId={UserId}", request.UserId);

                return new ApiResponse<string>("Successfully Logged out.",(int)ApiStatusCode.OK);
            }

            _logger.LogWarning("Logout failed for UserId={UserId}. No Active Session Found.", request.UserId);

            return new ApiResponse<string>("Logout failed. No Active Session Found.",(int)ApiStatusCode.NOT_FOUND);
        }
    }
}
