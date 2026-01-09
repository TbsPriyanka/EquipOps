using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.EquipmentCategory;

namespace EquipOps.BAL.Services
{
    public class EquipmentCategoryService(IEquipmentCategoryRepository _repository) : IEquipmentCategoryService
    {
        public async Task<ApiResponse<EquipmentCategoryResponseViewModel>> EquipmentCategoryCreateAsync(EquipmentCategoryRequest model)
        {
            if (model == null)
                return new ApiResponse<EquipmentCategoryResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Request model is null."
                };

            var data = await _repository.EquipmentCategoryCreateAsync(model);

            if (data == null || data.category_id <= 0)
                return new ApiResponse<EquipmentCategoryResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Invalid data."
                };

            string message = model.category_id == null
                ? "Equipment category has been inserted successfully."
                : "Equipment category has been updated successfully.";

            return new ApiResponse<EquipmentCategoryResponseViewModel>
            {
                StatusCode = (int)ApiStatusCode.OK,
                Success = true,
                Message = message,
                Data = data
            };
        }

        public async Task<ApiResponse<EquipmentCategoryResponse>> EquipmentCategoryListAsync(string? search, int length, int page, string orderColumn, string orderDirection)
        {
            var data = await _repository.EquipmentCategoryListAsync(search, length, page, orderColumn, orderDirection);

            return new ApiResponse<EquipmentCategoryResponse>
            {
                StatusCode = data == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data != null,
                Message = data == null ? "Invalid Data." : "Success.",
                Data = data
            };
        }

        public async Task<ApiResponse<EquipmentCategoryDeleteRequestViewModel>> EquipmentCategoryDeleteAsync(EquipmentCategoryDeleteRequestViewModel model)
        {
            var data = await _repository.EquipmentCategoryDeleteAsync(model);

            return new ApiResponse<EquipmentCategoryDeleteRequestViewModel>
            {
                StatusCode = data?.category_id == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data?.category_id != null,
                Message = data?.category_id == null ? "Invalid Data." : "Equipment category deleted successfully.",
                Data = data
            };
        }

        public async Task<ApiResponse<EquipmentCategoryResponseViewModel>> EquipmentCategoryByIdAsync(int? category_id)
        {
            if (category_id == null)
                return new ApiResponse<EquipmentCategoryResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Category Id cannot be empty."
                };

            var data = await _repository.EquipmentCategoryByIdAsync(category_id);

            return new ApiResponse<EquipmentCategoryResponseViewModel>
            {
                StatusCode = data == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data != null,
                Message = data == null ? "Invalid Data." : "Success.",
                Data = data
            };
        }
    }
}
