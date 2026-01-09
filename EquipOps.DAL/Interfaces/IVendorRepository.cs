using EquipOps.Model.Vendor;

namespace EquipOps.DAL.Interfaces
{
    public interface IVendorRepository
    {
        Task<VendorResponseViewModel> VendorCreateAsync(VendorRequest request);
        Task<VendorResponse> VendorListAsync(string? search, int length, int page, string orderColumn, string orderDirection);
        Task<VendorResponseViewModel> VendorByIdAsync(int? vendor_id);
        Task<VendorDeleteResponseViewModel> VendorDeleteAsync(VendorDeleteRequestViewModel request);
    }
}
