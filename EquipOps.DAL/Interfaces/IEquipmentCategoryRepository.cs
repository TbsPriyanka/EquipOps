using EquipOps.Model.EquipmentCategory;

namespace EquipOps.DAL.Interfaces
{
    public interface IEquipmentCategoryRepository
    {
        Task<EquipmentCategoryResponseViewModel> EquipmentCategoryCreateAsync(EquipmentCategoryRequest request);
        Task<EquipmentCategoryResponse> EquipmentCategoryListAsync(string? search, int length, int page, string orderColumn, string orderDirection);
        Task<EquipmentCategoryDeleteRequestViewModel> EquipmentCategoryDeleteAsync(EquipmentCategoryDeleteRequestViewModel request);
        Task<EquipmentCategoryResponseViewModel> EquipmentCategoryByIdAsync(int? category_id);
    }
}
