using EquipOps.BAL.Interfaces;
using EquipOps.Model.EquipmentCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipOps.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class EquipmentCategoryController(IEquipmentCategoryService _service) : ControllerBase
    {
        [HttpPost("createupdate")]
        public async Task<IActionResult> Create([FromBody] EquipmentCategoryRequest request)
        {
            var result = await _service.EquipmentCategoryCreateAsync(request);
            return Ok(result);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList(string? search = "", int length = 10, int page = 1, string orderColumn = "category_name", string orderDirection = "ASC")
        {
            var result = await _service.EquipmentCategoryListAsync(search, length, page, orderColumn, orderDirection);
            return Ok(result);
        }

        [HttpGet("getbyId")]
        public async Task<IActionResult> GetById(int? category_id)
        {
            var result = await _service.EquipmentCategoryByIdAsync(category_id);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] EquipmentCategoryDeleteRequestViewModel request)
        {
            var result = await _service.EquipmentCategoryDeleteAsync(request);
            return Ok(result);
        }
    }
}
