using EquipOps.Common.Helper;
using EquipOps.Model.Role;

namespace EquipOps.BAL.Interfaces
{
    public interface IUserRoleService
    {
        Task<ApiResponse<UserRoleResponseViewModel>> UserRoleCreateAsync(UserRoleRequest model);
        Task<ApiResponse<UserRoleResponse>> UserRoleListAsync(string? search, bool? Is_Active, int length, int page, string orderColumn, string orderDirection);
        Task<ApiResponse<UserRoleDeleteResponseViewModel>> UserRoleDeleteAsync(UserRoleDeleteRequestViewModel model);
        Task<ApiResponse<UserRoleResponseViewModel>> UserRoleByIdAsync(Guid? id);
    }
}
