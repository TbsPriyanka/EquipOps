using EquipOps.Model.Entities;
using EquipOps.Model.Requests.Equipment;
using EquipOps.Model.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipOps.DAL.Interfaces
{
	public interface IEquipmentRepository
	{
		Task<bool> AddOrUpdateAsync(EquipmentRequest request);
		Task<EquipmentEntity?> GetByIdAsync(int equipmentId);
		Task<IReadOnlyList<EquipmentDisplayDto>> GetAllAsync(int page, int size);
		Task<bool> DeleteAsync(int equipmentId);
	}
}
