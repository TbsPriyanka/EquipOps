using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.User;
using Microsoft.Extensions.Logging;

namespace EquipOps.BAL.Services
{
    public class UserService(ILogger<UserService> _logger, IUserRepository _userRepository) : IUserService
    {
        public async Task<ApiResponse<UserCreateUpdateResponse>> CreateUpdateUserAsync(UserCreateUpdateRequest? model)
        {
            if (model == null)
            {
                _logger.LogWarning("UserService: CreateUpdate Failed. Request Model Is Null.");

                return new ApiResponse<UserCreateUpdateResponse>
                {
                    Success = false,
                    Message = "Invalid Request."
                };
            }

            _logger.LogInformation("UserService: CreateUpdate START. Email={Email}", model.Email);

            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.FirstName))
            {
                _logger.LogWarning(
                    "Validation Failed: Required Fields Missing. Email={Email}",
                    model.Email);

                return new ApiResponse<UserCreateUpdateResponse>
                {
                    Success = false,
                    Message = "Email and First Name Are Required."
                };
            }

            _logger.LogInformation(
                "Calling UserRepository.CreateUpdate for Email={Email}", model.Email);

            var data = await _userRepository.AddOrUpdateUser(model);

            if (data == null || data.Id == Guid.Empty)
            {
                _logger.LogWarning(
                    "Create/Update Failed. No User returned. Email={Email}",
                    model.Email);

                return new ApiResponse<UserCreateUpdateResponse>
                {
                    Success = false,
                    Message = "User Create/Update Failed.",
                    Data = data
                };
            }

            _logger.LogInformation(
                "User Create/Update Successful. UserId={UserId}, Email={Email}",
                data.Id, data.Email);

            return new ApiResponse<UserCreateUpdateResponse>
            {
                Success = true,
                Message = model.Id == Guid.Empty
                    ? "User Created Successfully."
                    : "User Updated Successfully.",
                Data = data
            };
        }
        public async Task<ApiResponse<UserGetResponse>> GetUserByIdAsync(Guid Id)
        {
            _logger.LogInformation(
                "UserService: Fetching user. Id={Id}", Id);

            var data = await _userRepository.GetByIdAsync(Id);

            return new ApiResponse<UserGetResponse>
            {
                Success = true,
                Message = "User Fetched Successfully.",
                Data = data
            };
        }
        public async Task<ApiResponse<UserListResponseViewModel>> GetUserListAsync(string? Search, int Length, int Page, string OrderColumn, string OrderDirection = "Asc", string role = "", bool? isActive = null)
        {
            _logger.LogInformation(
                "UserService: Fetching user list. Search={Search}, Page={Page}, Length={Length}",
                Search, Page, Length);

            var data = await _userRepository.GetUserListAsync(Search, Length, Page, OrderColumn, OrderDirection, role, isActive);

            return new ApiResponse<UserListResponseViewModel>
            {
                Success = true,
                Message = "User List Fetched Successfully.",
                Data = data
            };
        }
        public async Task<ApiResponse<UserListResponseViewModel>> DeleteUserAsync(UserDeleteRequest request)
        {
            _logger.LogInformation("UserService: Deleting User With UserId={UserId}", request.Id);

            await _userRepository.DeleteUserAsync(request);

            var updatedList = await _userRepository.GetUserListAsync(
                Search: null,
                Length: 10,
                Page: 1,
                OrderColumn: "first_name",
                OrderDirection: "Asc"
            );
            return new ApiResponse<UserListResponseViewModel>
            {
                Success = true,
                Message = "User Deleted Successfully.",
                Data = updatedList
            };
        }
    }
}