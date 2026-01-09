using EquipOps.Common.Helper;
using EquipOps.Model.Requests.Equipment;
using EquipOps.Model.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipOps.BAL.Interfaces
{
	public interface IEquipmentService
	{
		Task<ApiResponse<bool>> AddOrUpdateAsync(EquipmentRequest request);
		Task<ApiResponse<EquipmentDisplayDto>> GetByIdAsync(int equipmentId);
		Task<ApiResponse<IReadOnlyList<EquipmentDisplayDto>>> GetAllAsync(int page, int size);
		Task<ApiResponse<bool>> DeleteAsync(int equipmentId);
	}
}
