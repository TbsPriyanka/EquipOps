using Dapper;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Entities;
using EquipOps.Model.Requests.Equipment;
using EquipOps.Model.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EquipOps.DAL.Repository
{
	public sealed class EquipmentRepository : IEquipmentRepository
	{
		private readonly IDbConnectionFactory _dbFactory;
		private readonly ILogger<EquipmentRepository> _logger;

		public EquipmentRepository(
			IDbConnectionFactory dbFactory,
			ILogger<EquipmentRepository> logger)
		{
			_dbFactory = dbFactory;
			_logger = logger;
		}

		public async Task<bool> AddOrUpdateAsync(EquipmentRequest request)
		{
			_logger.LogInformation(
				"Executing sp_equipment_add_update for EquipmentId={EquipmentId}",
				request.EquipmentId);

			using var conn = _dbFactory.CreateConnection();

			var parameters = new DynamicParameters();
			parameters.Add("p_equipment_id", request.EquipmentId, DbType.Int32);
			parameters.Add("p_organization_id", request.OrganizationId, DbType.Int32);
			parameters.Add("p_category_id", request.CategoryId, DbType.Int32);
			parameters.Add("p_name", request.Name, DbType.String);
			parameters.Add("p_type", request.Type, DbType.String);
			parameters.Add("p_qr_code", request.QrCode, DbType.String);
			parameters.Add("p_location", request.Location, DbType.String);

			// ⭐ IMPORTANT FIX — FORCE DATE
			parameters.Add(
				"p_purchase_date",
				request.PurchaseDate?.Date,
				DbType.Date
			);

			parameters.Add("p_status", request.Status, DbType.Int32);

			await conn.ExecuteAsync(
				"master.sp_equipment_add_update",
				parameters,
				commandType: CommandType.StoredProcedure
			);

			return true;
		}


		public async Task<EquipmentEntity?> GetByIdAsync(int equipmentId)
		{
			_logger.LogInformation(
				"Fetching Equipment By Id={EquipmentId}",
				equipmentId);

			using var conn = _dbFactory.CreateConnection();

			const string query = @"SELECT * FROM master.sp_equipment_get_by_id(@p_equipment_id)";

			var data = await conn.QueryFirstOrDefaultAsync<EquipmentEntity>(
				query,
				new { p_equipment_id = equipmentId }
			);

			if (data == null)
			{
				_logger.LogWarning(
					"Equipment not found for Id={EquipmentId}",
					equipmentId);
			}

			return data;
		}

		public async Task<IReadOnlyList<EquipmentDisplayDto>> GetAllAsync(int page, int size)
		{
			_logger.LogInformation(
				"Fetching Equipment List Page={Page}, Size={Size}",
				page, size);

			using var conn = _dbFactory.CreateConnection();

			const string query = @"SELECT * FROM master.sp_equipment_get_all(@p_page, @p_size)";

			var list = await conn.QueryAsync<EquipmentDisplayDto>(
				query,
				new
				{
					p_page = page,
					p_size = size
				});

			return list.AsList();
		}

		public async Task<bool> DeleteAsync(int equipmentId)
		{
			_logger.LogInformation(
				"Executing sp_equipment_delete for EquipmentId={EquipmentId}",
				equipmentId);

			using var conn = _dbFactory.CreateConnection();

			await conn.ExecuteAsync(
				"master.sp_equipment_delete",
				new { p_equipment_id = equipmentId },
				commandType: CommandType.StoredProcedure
			);

			return true;
		}
	}
}
