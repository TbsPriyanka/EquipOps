using EquipOps.BAL.Interfaces;
using EquipOps.Model.EquipmentFailure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipOps.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [AllowAnonymous]

    public class EquipmentFailureController(IEquipmentFailureService _EquipmentFailureService) : ControllerBase
    {
        [HttpPost("EquipmentFailureCreateUpdate")]
        public async Task<IActionResult> EquipmentFailureCreate([FromBody] EquipmentFailureRequest request)
        {
            var result = await _EquipmentFailureService.EquipmentFailureCreateAsync(request);
            return Ok(result);
        }

        [HttpGet("EquipmentFailureList")]
        public async Task<IActionResult> GetEquipmentFailureList(string? search = "", int length = 10, int page = 1, string orderColumn = "name", string orderDirection = "ASC")
        {
            var result = await _EquipmentFailureService.EquipmentFailureListAsync(
                search,
                length,
                page,
                orderColumn,
                orderDirection
            );
            return Ok(result);
        }

        [HttpGet("EquipmentFailureById")]
        public async Task<IActionResult> GetEquipmentFailureById(int? failure_id)
        {
            var result = await _EquipmentFailureService.EquipmentFailureByIdAsync(failure_id);
            return Ok(result);
        }

        [HttpPost("EquipmentFailureDelete")]
        public async Task<IActionResult> EquipmentFailureDelete([FromBody] EquipmentFailureDeleteRequestViewModel request)
        {
            var result = await _EquipmentFailureService.EquipmentFailureDeleteAsync(request);
            return Ok(result);
        }
    }
}