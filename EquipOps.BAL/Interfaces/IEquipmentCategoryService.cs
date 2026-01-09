using EquipOps.Common.Helper;
using EquipOps.Model.EquipmentCategory;

namespace EquipOps.BAL.Interfaces
{
    public interface IEquipmentCategoryService
    {
        Task<ApiResponse<EquipmentCategoryResponseViewModel>> EquipmentCategoryCreateAsync(EquipmentCategoryRequest model);
        Task<ApiResponse<EquipmentCategoryResponse>> EquipmentCategoryListAsync(string? search, int length, int page, string orderColumn, string orderDirection);
        Task<ApiResponse<EquipmentCategoryDeleteRequestViewModel>> EquipmentCategoryDeleteAsync(EquipmentCategoryDeleteRequestViewModel model);
        Task<ApiResponse<EquipmentCategoryResponseViewModel>> EquipmentCategoryByIdAsync(int? category_id);
    }
}
