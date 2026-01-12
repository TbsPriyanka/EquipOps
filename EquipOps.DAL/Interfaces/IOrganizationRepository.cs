using EquipOps.Model.Organization;

namespace EquipOps.DAL.Interfaces
{
    public interface IOrganizationRepository
    {
        Task<OrganizationResponseViewModel> OrganizationCreateAsync(OrganizationRequest request);
        Task<OrganizationResponse> OrganizationListAsync(string? search, int length, int page, string orderColumn, string orderDirection);
        Task<OrganizationResponseViewModel> OrganizationByIdAsync(int? organization_id);
        Task<OrganizationDeleteResponseViewModel> OrganizationDeleteAsync(OrganizationDeleteRequestViewModel request);
    }
}
