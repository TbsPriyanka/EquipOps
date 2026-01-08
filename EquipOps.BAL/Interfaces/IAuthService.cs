using EquipOps.Common.Helper;
using EquipOps.Model.AuthLogin;

namespace EquipOps.BAL.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthLoginResponseViewModel>> UserLoginAsync(AuthLoginRequestViewModel? model);
        Task<ApiResponse<ForgotPasswordAuthResponse>> ForgotPasswordAsync(string email);
        Task<ApiResponse<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<ApiResponse<string>> LogoutAsync(LogoutRequest request);
    }
}
