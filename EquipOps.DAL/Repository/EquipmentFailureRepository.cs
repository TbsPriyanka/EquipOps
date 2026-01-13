using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.EquipmentFailure;
using System.Data;

namespace EquipOps.DAL.Repository
{
    public class EquipmentFailureRepository(PgHelper _pghelper) : IEquipmentFailureRepository
    {
        public async Task<EquipmentFailureResponseViewModel> EquipmentFailureCreateAsync(EquipmentFailureRequest request)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_return_failure_id", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "p_return_updated_at", new DbParam { DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
                { "p_failure_id", new DbParam { Value = request.failure_id == 0 ? null : request.failure_id, DbType = DbType.Int32 } },
                { "p_organization_id", new DbParam { Value = request.organization_id, DbType = DbType.Int32 } },
                { "p_equipment_id", new DbParam { Value = request.equipment_id, DbType = DbType.Int32 } },
                { "p_subpart_id", new DbParam { Value = request.subpart_id, DbType = DbType.Int32 } },
                { "p_failure_date", new DbParam { Value = request.failure_date, DbType = DbType.DateTime } },
                { "p_failure_type", new DbParam { Value = request.failure_type, DbType = DbType.String } },
                { "p_description", new DbParam { Value = request.description, DbType = DbType.String } },
                { "p_downtime_minutes", new DbParam { Value = request.downtime_minutes, DbType = DbType.Int32 } }
            };

            var result = await _pghelper.CreateUpdateAsync("master.sp_equipment_failure_create_update", param);

            return new EquipmentFailureResponseViewModel
            {
                failure_id = result.p_return_failure_id,
                organization_id = request.organization_id,
                equipment_id = request.equipment_id,
                subpart_id = request.subpart_id,
                failure_date = request.failure_date,
                failure_type = request.failure_type,
                description = request.description,
                downtime_minutes = request.downtime_minutes,
                updated_date = result.p_return_updated_at
            };
        }

        public async Task<EquipmentFailureResponse> EquipmentFailureListAsync(string? search, int length, int page, string orderColumn, string orderDirection)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_search", new DbParam { Value = search ?? "", DbType = DbType.String } },
                { "p_length", new DbParam { Value = length, DbType = DbType.Int32 } },
                { "p_page", new DbParam { Value = page, DbType = DbType.Int32 } },
                { "p_order_column", new DbParam { Value = orderColumn, DbType = DbType.String } },
                { "p_order_direction", new DbParam { Value = orderDirection, DbType = DbType.String } },
                { "o_total_records", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "ref", new DbParam { Value = "mycursor", DbType = DbType.String, Direction = ParameterDirection.InputOutput } }
            };

            dynamic response = await _pghelper.ListAsync("master.sp_equipment_failure_list", param);

            EquipmentFailureResponse list = new();
            list.TotalNumbers = response.o_total_records;

            foreach (var row in response.@ref)
            {
                list.equipmentFailureResponseViewModel.Add(new EquipmentFailureResponseViewModel
                {
                    failure_id = row.failure_id,
                    organization_id = row.organization_id,
                    organization_name = row.organization_name,
                    equipment_id = row.equipment_id,
                    equipment_name = row.equipment_name,
                    subpart_id = row.subpart_id,
                    subpart_name = row.subpart_name,
                    failure_date = row.failure_date,
                    failure_type = row.failure_type,
                    description = row.description,
                    downtime_minutes = row.downtime_minutes,
                    created_date = row.created_date,
                    updated_date = row.updated_date
                });
            }

            return list;
        }

        public async Task<EquipmentFailureDeleteResponseViewModel> EquipmentFailureDeleteAsync(EquipmentFailureDeleteRequestViewModel request)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_return_failure_id", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "p_failure_id", new DbParam { Value = request.failure_id, DbType = DbType.Int32 } }
            };

            dynamic result = await _pghelper.CreateUpdateAsync("master.sp_equipment_failure_delete", param);

            return new EquipmentFailureDeleteResponseViewModel
            {
                failure_id = result.p_return_failure_id
            };
        }

        public async Task<EquipmentFailureResponseViewModel> EquipmentFailureByIdAsync(int? failure_id)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_failure_id", new DbParam { Value = failure_id, DbType = DbType.Int32 } },
                { "ref", new DbParam { Value = "mycursor", DbType = DbType.String, Direction = ParameterDirection.InputOutput } }
            };

            dynamic response = await _pghelper.ListAsync("master.sp_equipment_failure_getbyid", param);
            dynamic obj = response.@ref[0];

            return new EquipmentFailureResponseViewModel
            {
                failure_id = obj.failure_id,
                organization_id = obj.organization_id,
                equipment_id = obj.equipment_id,
                subpart_id = obj.subpart_id,
                failure_date = obj.failure_date,
                failure_type = obj.failure_type,
                description = obj.description,
                downtime_minutes = obj.downtime_minutes,
                created_date = obj.created_at
            };
        }
    }
}
