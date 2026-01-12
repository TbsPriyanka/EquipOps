using EquipOps.Common.Helper;
using EquipOps.Model.Entities;
using EquipOps.Model.Requests.Equipment;
using EquipOps.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipOps.API.Services.Interface
{
	public interface IEquipmentService
	{
		Task<ApiResponse<bool>> AddOrUpdateAsync(EquipmentRequest request);
		Task<ApiResponse<EquipmentDisplayDto>> GetByIdAsync(int equipmentId);
		Task<ApiResponse<EquipmentListResponseViewModel>> GetEquipmentsAsync(
		string? search,
		int length,
		int page,
		string orderColumn,
		string orderDirection
	);
		Task<ApiResponse<bool>> DeleteAsync(int equipmentId);
	}
}
