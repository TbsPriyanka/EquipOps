using EquipOps.Model.AuthLogin;

namespace EquipOps.DAL.Interfaces
{
    public interface IAuthRepository
    {
        Task<AuthLoginResponseViewModel> UserLoginAsync(AuthLoginRequestViewModel request);
        Task<bool> AddUserToken(UserTokenRequestViewModel user);
        Task<UserResponse?> GetUserByEmailAsync(string email);
        Task<bool> SaveResetTokenAsync(UserResponse userInfo, string resetToken);
        Task<UserTokenResponse?> GetUserByResetTokenAsync(string token);
        Task<bool> UpdatePasswordAsync(string token, string hashedPassword, UserTokenResponse user);
        Task<bool> ChangePasswordAsync(Guid userId, string hashedPassword);
        Task<bool> LogoutAsync(Guid? userId);
    }
}
