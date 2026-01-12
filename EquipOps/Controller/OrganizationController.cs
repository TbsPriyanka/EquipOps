using EquipOps.BAL.Interfaces;
using EquipOps.Model.Organization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipOps.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OrganizationController(IOrganizationService _organizationService) : ControllerBase
    {
        [HttpPost("organizationCreate")]
        public async Task<IActionResult> OrganizationCreate([FromBody] OrganizationRequest request)
        {
            var result = await _organizationService.OrganizationCreateAsync(request);
            return Ok(result);
        }

        [HttpGet("organizationList")]
        public async Task<IActionResult> GetOrganizationList(string? search = "", int length = 10, int page = 1, string orderColumn = "name", string orderDirection = "ASC")
        {
            var result = await _organizationService.OrganizationListAsync(search, length, page, orderColumn, orderDirection);
            return Ok(result);
        }

        [HttpGet("organizationById")]
        public async Task<IActionResult> GetOrganizationById(int? organization_id)
        {
            var result = await _organizationService.OrganizationByIdAsync(organization_id);
            return Ok(result);
        }

        [HttpDelete("organizationDelete")]
        public async Task<IActionResult> OrganizationDelete([FromBody] OrganizationDeleteRequestViewModel request)
        {
            var result = await _organizationService.OrganizationDeleteAsync(request);
            return Ok(result);
        }
    }
}
