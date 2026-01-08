using EquipOps.Model.Role;

namespace EquipOps.DAL.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<UserRoleResponseViewModel> UserRoleCreateAsync(UserRoleRequest request);
        Task<UserRoleResponse> UserRoleListAsync(string? search, bool? IsActive, int length, int page, string orderColumn, string orderDirection);
        Task<UserRoleDeleteResponseViewModel> UserRoleDeleteAsync(UserRoleDeleteRequestViewModel request);
        Task<UserRoleResponseViewModel> UserRoleByIdAsync(Guid? id);
    }
}