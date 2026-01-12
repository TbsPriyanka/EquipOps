using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.EquipmentFailure;

namespace EquipOps.BAL.Services
{
    public class EquipmentFailureService(IEquipmentFailureRepository _equipmentFailureRepository) : IEquipmentFailureService
    {
        public async Task<ApiResponse<EquipmentFailureResponseViewModel>> EquipmentFailureCreateAsync(EquipmentFailureRequest model)
        {
            if (model == null)
            {
                return new ApiResponse<EquipmentFailureResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Request model is null."
                };
            }

            var data = await _equipmentFailureRepository.EquipmentFailureCreateAsync(model);

            if (data == null || data.failure_id <= 0)
            {
                return new ApiResponse<EquipmentFailureResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Invalid data."
                };
            }

            string message = model.failure_id == null
                ? "Equipment failure has been inserted successfully."
                : "Equipment failure has been updated successfully.";

            return new ApiResponse<EquipmentFailureResponseViewModel>
            {
                StatusCode = (int)ApiStatusCode.OK,
                Success = true,
                Message = message,
                Data = data
            };
        }

        public async Task<ApiResponse<EquipmentFailureResponse>> EquipmentFailureListAsync(string? search,int length,int page,string orderColumn,string orderDirection)
        {
            var data = await _equipmentFailureRepository
                .EquipmentFailureListAsync(search, length, page, orderColumn, orderDirection);

            return new ApiResponse<EquipmentFailureResponse>
            {
                StatusCode = data == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data != null,
                Message = data == null ? "Invalid Data." : "Success.",
                Data = data
            };
        }

        public async Task<ApiResponse<EquipmentFailureDeleteResponseViewModel>> EquipmentFailureDeleteAsync(EquipmentFailureDeleteRequestViewModel model)
        {
            var data = await _equipmentFailureRepository.EquipmentFailureDeleteAsync(model);

            return new ApiResponse<EquipmentFailureDeleteResponseViewModel>
            {
                StatusCode = data?.failure_id == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data?.failure_id != null,
                Message = data?.failure_id == null
                    ? "Invalid Data."
                    : "Equipment failure deleted successfully.",
                Data = data
            };
        }

        public async Task<ApiResponse<EquipmentFailureResponseViewModel>> EquipmentFailureByIdAsync(int? failure_id)
        {
            if (failure_id == null)
            {
                return new ApiResponse<EquipmentFailureResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Failure Id cannot be empty."
                };
            }

            var data = await _equipmentFailureRepository.EquipmentFailureByIdAsync(failure_id);

            return new ApiResponse<EquipmentFailureResponseViewModel>
            {
                StatusCode = data == null ? (int)ApiStatusCode.BAD_REQUEST : (int)ApiStatusCode.OK,
                Success = data != null,
                Message = data == null ? "Invalid Data." : "Success.",
                Data = data
            };
        }
    }
}
