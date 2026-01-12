using EquipOps.API.Services.Interface;
using EquipOps.BAL.Interfaces;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.DAL.Repository;
using EquipOps.Model.Entities;
using EquipOps.Model.Requests.Equipment;
using EquipOps.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipOps.API.Services.Implementation
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

		public async Task<ApiResponse<EquipmentListResponseViewModel>> GetEquipmentsAsync(
	   string? search,
	   int length,
	   int page,
	   string orderColumn,
	   string orderDirection)
		{
			try
			{
				_logger.LogInformation("BAL GetEquipmentsAsync called");

				var data = await _repository.GetEquipmentAsync(
					search,
					length,
					page,
					orderColumn,
					orderDirection
				);

				if (data == null || data.EquipmentData == null || !data.EquipmentData.Any())
				{
					return new ApiResponse<EquipmentListResponseViewModel>(
						message: "Equipments not found.",
						statusCode: (int)ApiStatusCode.NOT_FOUND
					);
				}

				return new ApiResponse<EquipmentListResponseViewModel>(
					data,
					"Equipments retrieved successfully.",
					(int)ApiStatusCode.OK
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "BAL GetEquipmentsAsync error");

				return new ApiResponse<EquipmentListResponseViewModel>(
					message: "Internal server error",
					statusCode: (int)ApiStatusCode.INTERNAL_SERVER_ERROR
				);
			}
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
