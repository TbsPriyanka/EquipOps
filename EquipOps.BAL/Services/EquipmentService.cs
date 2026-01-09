using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Requests.Equipment;
using EquipOps.Model.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipOps.BAL.Services
{
	public sealed class EquipmentService : IEquipmentService
	{
		private readonly IEquipmentRepository _repository;
		private readonly ILogger<EquipmentService> _logger;

		public EquipmentService(
			IEquipmentRepository repository,
			ILogger<EquipmentService> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task<ApiResponse<bool>> AddOrUpdateAsync(EquipmentRequest request)
		{
			_logger.LogInformation("BAL AddOrUpdateAsync called");

			await _repository.AddOrUpdateAsync(request);

			return new ApiResponse<bool>(
				data: true,
				message: "Equipment saved successfully",
				statusCode: (int)ApiStatusCode.OK
			);
		}

		public async Task<ApiResponse<EquipmentDisplayDto>> GetByIdAsync(int equipmentId)
		{
			_logger.LogInformation(
				"BAL GetByIdAsync EquipmentId={EquipmentId}",
				equipmentId);

			var entity = await _repository.GetByIdAsync(equipmentId);

			if (entity == null)
			{
				return new ApiResponse<EquipmentDisplayDto>(
					message: "Equipment not found",
					statusCode: (int)ApiStatusCode.NOT_FOUND
				);
			}

			var dto = new EquipmentDisplayDto
			{
				EquipmentId = entity.EquipmentId,
				OrganizationId = entity.OrganizationId,
				CategoryId = entity.CategoryId,
				Name = entity.Name,
				Type = entity.Type,
				QrCode = entity.QrCode,
				Location = entity.Location,
				PurchaseDate = entity.PurchaseDate,
				Status = entity.Status,
				TotalCount = 1
			};

			return new ApiResponse<EquipmentDisplayDto>(
				dto,
				"Success",
				(int)ApiStatusCode.OK
			);
		}

		public async Task<ApiResponse<IReadOnlyList<EquipmentDisplayDto>>> GetAllAsync(int page, int size)
		{
			_logger.LogInformation("BAL GetAllAsync");

			var data = await _repository.GetAllAsync(page, size);

			return new ApiResponse<IReadOnlyList<EquipmentDisplayDto>>(
				data,
				"Success",
				(int)ApiStatusCode.OK
			);
		}

		public async Task<ApiResponse<bool>> DeleteAsync(int equipmentId)
		{
			_logger.LogInformation(
				"BAL DeleteAsync EquipmentId={EquipmentId}",
				equipmentId);

			await _repository.DeleteAsync(equipmentId);

			return new ApiResponse<bool>(
				true,
				"Equipment deleted successfully",
				(int)ApiStatusCode.OK
			);
		}
	}
}
