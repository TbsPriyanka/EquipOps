using EquipOps.Common.Helper;
using EquipOps.Model.Organization;

namespace EquipOps.BAL.Interfaces
{
    public interface IOrganizationService
    {
        Task<ApiResponse<OrganizationResponseViewModel>> OrganizationCreateAsync(OrganizationRequest model);
        Task<ApiResponse<OrganizationResponse>> OrganizationListAsync(string? search, int length, int page, string orderColumn, string orderDirection);
        Task<ApiResponse<OrganizationResponseViewModel>> OrganizationByIdAsync(int? organization_id);
        Task<ApiResponse<OrganizationDeleteResponseViewModel>> OrganizationDeleteAsync(OrganizationDeleteRequestViewModel model);
    }
}
