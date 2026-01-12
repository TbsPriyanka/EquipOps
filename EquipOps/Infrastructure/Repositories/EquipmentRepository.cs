using Dapper;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Entities;
using EquipOps.Model.Requests.Equipment;
using EquipOps.Model.Responses;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
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

			using var conn = (NpgsqlConnection)_dbFactory.CreateConnection();
			await conn.OpenAsync();

			using var tran = await conn.BeginTransactionAsync();

			try
			{
				await using var cmd = new NpgsqlCommand(
					"master.sp_equipment_get_by_id",
					conn,
					(NpgsqlTransaction)tran
				);

				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("p_equipment_id", equipmentId);

				var cursorParam = new NpgsqlParameter("ref", NpgsqlDbType.Refcursor)
				{
					Direction = ParameterDirection.InputOutput,
					Value = "equipment_by_id_cursor"
				};
				cmd.Parameters.Add(cursorParam);

				// Execute procedure
				await cmd.ExecuteNonQueryAsync();

				// Fetch from cursor
				var data = await conn.QueryFirstOrDefaultAsync<EquipmentEntity>(
					"FETCH ALL FROM \"equipment_by_id_cursor\"",
					transaction: tran
				);

				await tran.CommitAsync();

				if (data == null)
				{
					_logger.LogWarning(
						"Equipment not found for Id={EquipmentId}",
						equipmentId);
				}

				return data;
			}
			catch (Exception ex)
			{
				await tran.RollbackAsync();
				_logger.LogError(ex, "Error fetching equipment by id {EquipmentId}", equipmentId);
				throw;
			}
		}


		public async Task<EquipmentListResponseViewModel> GetEquipmentAsync(
	string? search,
	int length,
	int page,
	string orderColumn,
	string orderDirection)
		{
			using var conn = (NpgsqlConnection)_dbFactory.CreateConnection();
			await conn.OpenAsync();

			using var tran = await conn.BeginTransactionAsync();

			try
			{
				await using var cmd = new NpgsqlCommand(
					"master.sp_equipment_list_get",
					conn,
					(NpgsqlTransaction)tran
				);

				cmd.CommandType = CommandType.StoredProcedure;

				// 🔹 IN parameters
				cmd.Parameters.AddWithValue("p_search", (object?)search ?? DBNull.Value);
				cmd.Parameters.AddWithValue("p_length", length);
				cmd.Parameters.AddWithValue("p_page", page);
				cmd.Parameters.AddWithValue("p_order_column", orderColumn);
				cmd.Parameters.AddWithValue("p_order_direction", orderDirection);

				// 🔹 INOUT total count
				var totalParam = new NpgsqlParameter("o_total_numbers", NpgsqlDbType.Integer)
				{
					Direction = ParameterDirection.InputOutput,
					Value = 0
				};
				cmd.Parameters.Add(totalParam);

				// 🔹 INOUT refcursor
				var cursorParam = new NpgsqlParameter("ref", NpgsqlDbType.Refcursor)
				{
					Direction = ParameterDirection.InputOutput,
					Value = "equipment_cursor"
				};
				cmd.Parameters.Add(cursorParam);

				// ✅ Execute SP
				await cmd.ExecuteNonQueryAsync();

				// ✅ Fetch cursor (same transaction)
				var data = (await conn.QueryAsync<EquipmentDisplayDto>(
					"FETCH ALL FROM \"equipment_cursor\"",
					transaction: tran
				)).ToList();

				await tran.CommitAsync();

				return new EquipmentListResponseViewModel
				{
					TotalNumbers = (int)totalParam.Value,
					EquipmentData = data
				};
			}
			catch (Exception ex)
			{
				await tran.RollbackAsync();
				_logger.LogError(ex, "Error in EquipmentRepository.GetEquipmentAsync");
				throw;
			}
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
