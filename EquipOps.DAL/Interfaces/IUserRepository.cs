using EquipOps.Model.User;

namespace EquipOps.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<UserCreateUpdateResponse> AddOrUpdateUser(UserCreateUpdateRequest request);
        Task<UserGetResponse?> GetByIdAsync(Guid userId);
        Task<UserListResponseViewModel> GetUserListAsync(string? Search, int Length, int Page, string OrderColumn, string OrderDirection = "Asc", string role = "", bool? isActive = null);
        Task DeleteUserAsync(UserDeleteRequest request);
    }
}
