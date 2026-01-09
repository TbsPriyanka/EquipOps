using EquipOps.Common.Helper;
using EquipOps.Model.Vendor;

namespace EquipOps.BAL.Interfaces
{
    public interface IVendorService
    {
        Task<ApiResponse<VendorResponseViewModel>> VendorCreateAsync(VendorRequest model);
        Task<ApiResponse<VendorResponse>> VendorListAsync(string? search, int length, int page, string orderColumn, string orderDirection);
        Task<ApiResponse<VendorResponseViewModel>> VendorByIdAsync(int? vendor_id);
        Task<ApiResponse<VendorDeleteResponseViewModel>> VendorDeleteAsync(VendorDeleteRequestViewModel model);
    }
}
