using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Vendor;

namespace EquipOps.BAL.Services
{
    public class VendorService(IVendorRepository _vendorRepository) : IVendorService
    {
        public async Task<ApiResponse<VendorResponseViewModel>> VendorCreateAsync(VendorRequest model)
        {
            if (model == null)
            {
                return new ApiResponse<VendorResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Request model is null."
                };
            }

            var data = await _vendorRepository.VendorCreateAsync(model);

            if (data == null || data.vendor_id <= 0)
            {
                return new ApiResponse<VendorResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Invalid data."
                };
            }

            string message = model.vendor_id == null
                ? "Vendor has been inserted successfully."
                : "Vendor has been updated successfully.";

            return new ApiResponse<VendorResponseViewModel>
            {
                StatusCode = (int)ApiStatusCode.OK,
                Success = true,
                Message = message,
                Data = data
            };
        }

        public async Task<ApiResponse<VendorResponse>> VendorListAsync(string? search, int length, int page, string orderColumn, string orderDirection)
        {
            var data = await _vendorRepository
                .VendorListAsync(search, length, page, orderColumn, orderDirection);

            return new ApiResponse<VendorResponse>
            {
                StatusCode = data == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data != null,
                Message = data == null ? "Invalid Data." : "Success.",
                Data = data
            };
        }

        public async Task<ApiResponse<VendorDeleteResponseViewModel>> VendorDeleteAsync(VendorDeleteRequestViewModel model)
        {
            var data = await _vendorRepository.VendorDeleteAsync(model);

            return new ApiResponse<VendorDeleteResponseViewModel>
            {
                StatusCode = data?.vendor_id == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data?.vendor_id != null,
                Message = data?.vendor_id == null ? "Invalid Data." : "Vendor deleted successfully.",
                Data = data
            };
        }

        public async Task<ApiResponse<VendorResponseViewModel>> VendorByIdAsync(int? vendor_id)
        {
            if (vendor_id == null)
            {
                return new ApiResponse<VendorResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Vendor Id cannot be empty."
                };
            }

            var data = await _vendorRepository.VendorByIdAsync(vendor_id);

            return new ApiResponse<VendorResponseViewModel>
            {
                StatusCode = data == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data != null,
                Message = data == null ? "Invalid Data." : "Success.",
                Data = data
            };
        }
    }
}
