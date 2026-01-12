using EquipOps.Common.Helper;
using EquipOps.Model.EquipmentFailure;

namespace EquipOps.BAL.Interfaces
{
    public interface IEquipmentFailureService
    {
        Task<ApiResponse<EquipmentFailureResponseViewModel>> EquipmentFailureCreateAsync(EquipmentFailureRequest model);
        Task<ApiResponse<EquipmentFailureResponse>> EquipmentFailureListAsync(string? search, int length, int page, string orderColumn, string orderDirection);
        Task<ApiResponse<EquipmentFailureResponseViewModel>> EquipmentFailureByIdAsync(int? failure_id);
        Task<ApiResponse<EquipmentFailureDeleteResponseViewModel>> EquipmentFailureDeleteAsync(EquipmentFailureDeleteRequestViewModel model);
    }
}
