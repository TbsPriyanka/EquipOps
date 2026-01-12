using EquipOps.Model.EquipmentFailure;

namespace EquipOps.DAL.Interfaces
{
    public interface IEquipmentFailureRepository
    {
        Task<EquipmentFailureResponseViewModel> EquipmentFailureCreateAsync(EquipmentFailureRequest request);
        Task<EquipmentFailureResponse> EquipmentFailureListAsync(string? search, int length, int page, string orderColumn, string orderDirection);
        Task<EquipmentFailureResponseViewModel> EquipmentFailureByIdAsync(int? failure_id);
        Task<EquipmentFailureDeleteResponseViewModel> EquipmentFailureDeleteAsync(EquipmentFailureDeleteRequestViewModel request);
    }
}
