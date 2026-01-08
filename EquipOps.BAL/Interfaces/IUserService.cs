using EquipOps.Common.Helper;
using EquipOps.Model.User;

namespace EquipOps.BAL.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserCreateUpdateResponse>> CreateUpdateUserAsync(UserCreateUpdateRequest model);
        Task<ApiResponse<UserGetResponse>> GetUserByIdAsync(Guid Id);
        Task<ApiResponse<UserListResponseViewModel>> GetUserListAsync(string? Search, int Length, int Page, string OrderColumn, string OrderDirection = "Asc", string role = "", bool? isActive = null);
        Task<ApiResponse<UserListResponseViewModel>> DeleteUserAsync(UserDeleteRequest request);
    }
}
