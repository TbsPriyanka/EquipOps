using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Role;

namespace EquipOps.BAL.Services
{
    public class UserRoleService(IUserRoleRepository _userRoleRepository) : IUserRoleService
    {
        public async Task<ApiResponse<UserRoleResponseViewModel>> UserRoleCreateAsync(UserRoleRequest model)
        {
            var data = await _userRoleRepository.UserRoleCreateAsync(model);

            string Message = "";
            bool Status = false;
            int Code = 0;
            if (data.id == null)
            {
                Code = (int)ApiStatusCode.BAD_REQUEST;
                Message = "Invalid data";
            }
            else
            {
                Code = (int)ApiStatusCode.OK;
                Status = true;
                if (model.id == null)
                    Message = "User Role Is Inserted Successfully.";
                else
                    Message = "User Role Is Updated Successfully.";
            }

            return new ApiResponse<UserRoleResponseViewModel>
            {
                StatusCode = Code,
                Success = Status,
                Message = Message,
                Data = data
            };
        }
        public async Task<ApiResponse<UserRoleResponse>> UserRoleListAsync(string? search,bool? Is_Active,int length,int page,string orderColumn,string orderDirection)
        {
            var data = await _userRoleRepository
                .UserRoleListAsync(search, Is_Active, length, page, orderColumn, orderDirection);

            int code;
            bool status;
            string message;

            if (data == null)
            {
                code = (int)ApiStatusCode.BAD_REQUEST;
                status = false;
                message = "Invalid Data.";
            }
            else
            {
                code = (int)ApiStatusCode.OK;
                status = true;
                message = "Success.";
            }

            return new ApiResponse<UserRoleResponse>
            {
                StatusCode = code,
                Success = status,
                Message = message,
                Data = data
            };
        }
        public async Task<ApiResponse<UserRoleDeleteResponseViewModel>> UserRoleDeleteAsync(UserRoleDeleteRequestViewModel model)
        {
            var data = await _userRoleRepository.UserRoleDeleteAsync(model);

            int code;
            bool status;
            string message;

            if (data == null || data.id == null)
            {
                code = (int)ApiStatusCode.BAD_REQUEST;
                status = false;
                message = "Invalid Data";
            }
            else
            {
                code = (int)ApiStatusCode.OK;
                status = true;
                message = "User Role Is Deleted Successfully.";
            }

            return new ApiResponse<UserRoleDeleteResponseViewModel>
            {
                StatusCode = code,
                Success = status,
                Message = message,
                Data = data
            };
        }

        public async Task<ApiResponse<UserRoleResponseViewModel>> UserRoleByIdAsync(Guid? id)
        {
            if (id == null)
            {
                return new ApiResponse<UserRoleResponseViewModel>
                {
                    StatusCode = (int)ApiStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = "Id Cannot Be Empty."
                };
            }

            var data = await _userRoleRepository.UserRoleByIdAsync(id);

            int code;
            bool status;
            string message;

            if (data == null)
            {
                code = (int)ApiStatusCode.BAD_REQUEST;
                status = false;
                message = "Invalid Data.";
            }
            else
            {
                code = (int)ApiStatusCode.OK;
                status = true;
                message = "Success.";
            }

            return new ApiResponse<UserRoleResponseViewModel>
            {
                StatusCode = code,
                Success = status,
                Message = message,
                Data = data
            };
        }
    }
}
