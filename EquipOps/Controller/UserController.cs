using EquipOps.API.Services.Interface;
using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipOps.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(ILogger<UserController> _logger, IUserService _userService) : ControllerBase
    {
        [HttpPost("createupdate")]
        public async Task<IActionResult> CreateOrUpdateUser(UserCreateUpdateRequest request)
        {
            _logger.LogInformation("API hit: CreateOrUpdateUser. Email={Email}", request.Email);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                _logger.LogWarning("Validation Failed For Email={Email}", request.Email);
                return BadRequest(ResponseHelper<string>.Error("Validation failed",errors: errors,statusCode: ApiStatusCode.BAD_REQUEST));
            }

            var result = await _userService.CreateUpdateUserAsync(request);
            _logger.LogInformation(
                "Service Response For Email={Email}: Success={Success}",
                request.Email,
                result.Success
            );

            if (!result.Success)
            {
                return Conflict(ResponseHelper<string>.Error(result.Message ?? "User Create/Update Failed.",statusCode: ApiStatusCode.CONFLICT_OCCURS));
            }

            if (result.Data == null)
            {
                return Conflict(ResponseHelper<string>.Error("User Create/Update Failed.",statusCode: ApiStatusCode.CONFLICT_OCCURS));
            }
            return Ok(ResponseHelper<UserCreateUpdateResponse>.Success(result.Message ?? "User Created Successfully.", result.Data));
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetUserById(Guid Id)
        {
            _logger.LogInformation(
                "API hit: GetUserById. Id={Id}", Id);
            var result = await _userService.GetUserByIdAsync(Id);

            if (!result.Success || result.Data == null)
            {
                return NotFound(ResponseHelper<string>.Error(result.Message ?? "No User Found.",statusCode: ApiStatusCode.NOT_FOUND));
            }
            return Ok(ResponseHelper<UserGetResponse>.Success("User Fetched Successfully.", result.Data));
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetUserList(string? search, int length = 10, int page = 1, string orderColumn = "name", string orderDirection = "Asc", string role = "", bool? is_active = null)
        {
            _logger.LogInformation(
                "API hit: GetUserList. Search={Search}, Page={Page}, Length={Length}",
                search, page, length);
            var result = await _userService.GetUserListAsync(
                search, length, page, orderColumn, orderDirection, role, is_active);

            if (!result.Success || result.Data == null)
            {
                return NotFound(ResponseHelper<string>.Error(result.Message ?? "No Users Found.",statusCode: ApiStatusCode.NOT_FOUND));
            }
            return Ok(ResponseHelper<UserListResponseViewModel>.Success("User List Fetched Successfully.", result.Data));
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] UserDeleteRequest request)
        {
            _logger.LogInformation("API hit: DeleteUser. UserId={UserId}", request.Id);

            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("Validation failed: UserId Is Required");
                return BadRequest(ResponseHelper<string>.Error(
                    "UserId Is Required",
                    statusCode: ApiStatusCode.BAD_REQUEST
                ));
            }

            var result = await _userService.DeleteUserAsync(request);

            if (!result.Success || result.Data == null)
            {
                return NotFound(ResponseHelper<string>.Error(result.Message ?? "User Deletion Failed Or User Not Found.",statusCode: ApiStatusCode.NOT_FOUND));
            }

            return Ok(ResponseHelper<UserListResponseViewModel>.Success(result.Message ?? "User Deleted Successfully.",result.Data));
        }
    }
}
