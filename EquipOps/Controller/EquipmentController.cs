using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.Model.Requests.Equipment;
using EquipOps.Model.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EquipOps.API.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class EquipmentController(
	ILogger<EquipmentController> _logger,
	IEquipmentService _equipmentService
) : ControllerBase
	{
		// ------------------------------------------------------------
		// Create or Update Equipment
		// ------------------------------------------------------------
		[AllowAnonymous]
		[HttpPost("createupdate")]
		public async Task<IActionResult> CreateOrUpdateEquipment(
			[FromBody] EquipmentRequest request)
		{
			_logger.LogInformation(
				"API hit: CreateOrUpdateEquipment. Name={Name}",
				request.Name
			);

			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				_logger.LogWarning(
					"Validation failed for Equipment Name={Name}",
					request.Name
				);

				return BadRequest(
					ResponseHelper<string>.Error(
						"Validation failed",
						errors: errors,
						statusCode: ApiStatusCode.BAD_REQUEST
					)
				);
			}

			var result = await _equipmentService.AddOrUpdateAsync(request);

			_logger.LogInformation(
				"Service response for Equipment Name={Name}, Success={Success}",
				request.Name,
				result.Success
			);

			if (!result.Success || result.Data == null)
			{
				return Conflict(
					ResponseHelper<string>.Error(
						result.Message ?? "Equipment Create/Update Failed.",
						statusCode: ApiStatusCode.CONFLICT_OCCURS
					)
				);
			}

			return Ok(
				ResponseHelper<bool>.Success(
					result.Message ?? "Equipment Saved Successfully.",
					result.Data
				)
			);
		}

		// ------------------------------------------------------------
		// Get Equipment By Id
		// ------------------------------------------------------------
		[HttpGet("getById")]
		public async Task<IActionResult> GetEquipmentById(int id)
		{
			_logger.LogInformation(
				"API hit: GetEquipmentById. EquipmentId={EquipmentId}",
				id
			);

			if (id <= 0)
			{
				_logger.LogWarning("Validation failed: EquipmentId is required");
				return BadRequest(
					ResponseHelper<string>.Error(
						"EquipmentId is required",
						statusCode: ApiStatusCode.BAD_REQUEST
					)
				);
			}

			var result = await _equipmentService.GetByIdAsync(id);

			if (!result.Success || result.Data == null)
			{
				return NotFound(
					ResponseHelper<string>.Error(
						result.Message ?? "Equipment Not Found.",
						statusCode: ApiStatusCode.NOT_FOUND
					)
				);
			}

			return Ok(
				ResponseHelper<EquipmentDisplayDto>.Success(
					"Equipment Fetched Successfully.",
					result.Data
				)
			);
		}

		// ------------------------------------------------------------
		// Get Equipment List
		// ------------------------------------------------------------
		[AllowAnonymous]
		[HttpGet("list")]
		public async Task<IActionResult> GetEquipmentList(
			int page = 1,
			int size = 10)
		{
			_logger.LogInformation(
				"API hit: GetEquipmentList. Page={Page}, Size={Size}",
				page,
				size
			);

			var result = await _equipmentService.GetAllAsync(page, size);

			if (!result.Success || result.Data == null)
			{
				return NotFound(
					ResponseHelper<string>.Error(
						result.Message ?? "No Equipment Found.",
						statusCode: ApiStatusCode.NOT_FOUND
					)
				);
			}

			return Ok(
				ResponseHelper<IReadOnlyList<EquipmentDisplayDto>>.Success(
					"Equipment List Fetched Successfully.",
					result.Data
				)
			);
		}

		// ------------------------------------------------------------
		// Delete Equipment
		// ------------------------------------------------------------
		[HttpDelete("delete")]
		public async Task<IActionResult> DeleteEquipment(
			[FromBody] EquipmentRequest request)
		{
			_logger.LogInformation(
				"API hit: DeleteEquipment. EquipmentId={EquipmentId}",
				request.EquipmentId
			);

			if (request.EquipmentId <= 0)
			{
				_logger.LogWarning("Validation failed: EquipmentId is required");

				return BadRequest(
					ResponseHelper<string>.Error(
						"EquipmentId is required",
						statusCode: ApiStatusCode.BAD_REQUEST
					)
				);
			}

			var result = await _equipmentService.DeleteAsync(request.EquipmentId);

			if (!result.Success || result.Data == null)
			{
				return NotFound(
					ResponseHelper<string>.Error(
						result.Message ?? "Equipment Deletion Failed or Not Found.",
						statusCode: ApiStatusCode.NOT_FOUND
					)
				);
			}

			return Ok(
				ResponseHelper<bool>.Success(
					result.Message ?? "Equipment Deleted Successfully.",
					result.Data
				)
			);
		}
	}
}
