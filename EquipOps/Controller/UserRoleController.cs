using EquipOps.BAL.Interfaces;
using EquipOps.Model.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipOps.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [AllowAnonymous]

    public class UserRoleController(IUserRoleService _userRoleService) : ControllerBase
    {
        [HttpPost("userRoleCreate")]
        public async Task<IActionResult> userRoleCreate([FromBody] UserRoleRequest request)
        {
            var result = await _userRoleService.UserRoleCreateAsync(request);
            return Ok(result);
        }

        [HttpGet("userRoleList")]
        public async Task<IActionResult> GetUserRoleList(string? search = "", bool? Is_Active = null, int length = 10, int page = 1, string orderColumn = "name", string orderDirection = "ASC")
        {
            var result = await _userRoleService.UserRoleListAsync(search, Is_Active, length, page, orderColumn, orderDirection);
            return Ok(result);
        }

        [HttpGet("userRoleById")]
        public async Task<IActionResult> GetUserRoleById(Guid? id)
        {
            var result = await _userRoleService.UserRoleByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete("userRoleDelete")]
        public async Task<IActionResult> userRoleDelete([FromBody] UserRoleDeleteRequestViewModel request)
        {
            var result = await _userRoleService.UserRoleDeleteAsync(request);
            return Ok(result);
        }
    }
}
