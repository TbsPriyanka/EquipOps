using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Organization;

namespace EquipOps.BAL.Services
{
    public class OrganizationService(IOrganizationRepository _organizationRepository) : IOrganizationService
    {
        public async Task<ApiResponse<OrganizationResponseViewModel>> OrganizationCreateAsync(OrganizationRequest model)
        {
            if (model == null)
            {
                return new ApiResponse<OrganizationResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Request model is null."
                };
            }

            var data = await _organizationRepository.OrganizationCreateAsync(model);

            string message = model.organization_id == null
                ? "Organization has been inserted successfully."
                : "Organization has been updated successfully.";

            return new ApiResponse<OrganizationResponseViewModel>
            {
                StatusCode = (int)ApiStatusCode.OK,
                Success = true,
                Message = message,
                Data = data
            };
        }

        public async Task<ApiResponse<OrganizationResponse>> OrganizationListAsync(string? search, int length, int page, string orderColumn, string orderDirection)
        {
            var data = await _organizationRepository.OrganizationListAsync(search, length, page, orderColumn, orderDirection);

            return new ApiResponse<OrganizationResponse>
            {
                StatusCode = (int)ApiStatusCode.OK,
                Success = true,
                Message = "Success",
                Data = data
            };
        }

        public async Task<ApiResponse<OrganizationResponseViewModel>> OrganizationByIdAsync(int? organization_id)
        {
            if (organization_id == null)
            {
                return new ApiResponse<OrganizationResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Organization Id cannot be empty."
                };
            }

            var data = await _organizationRepository.OrganizationByIdAsync(organization_id);

            return new ApiResponse<OrganizationResponseViewModel>
            {
                StatusCode = (int)ApiStatusCode.OK,
                Success = true,
                Message = "Success",
                Data = data
            };
        }

        public async Task<ApiResponse<OrganizationDeleteResponseViewModel>> OrganizationDeleteAsync(OrganizationDeleteRequestViewModel model)
        {
            var data = await _organizationRepository.OrganizationDeleteAsync(model);

            return new ApiResponse<OrganizationDeleteResponseViewModel>
            {
                StatusCode = (int)ApiStatusCode.OK,
                Success = true,
                Message = "Organization deleted successfully.",
                Data = data
            };
        }
    }
}
